using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace FastModule.Core;

public class ModuleDiscovery
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<Assembly, IReadOnlyList<Type>> _moduleTypeCache = new();

    private static readonly string[] ExcludedAssemblyPrefixes =
    {
        "System.",
        "Microsoft.",
        "Newtonsoft.",
        "Azure.",
        "WindowsBase",
        "mscorlib",
    };

    public ModuleDiscovery(ILogger logger)
    {
        _logger = logger;
    }

    public IEnumerable<Type> DiscoverModules()
    {
        var assemblies = GetRelevantAssemblies();
        Console.WriteLine("Found {0} assemblies", assemblies.Count());
        foreach (var assembly in assemblies)
        {
            Console.WriteLine("Assembly: {0}", assembly.FullName);
        }
        return assemblies.SelectMany(GetModuleTypes).Distinct();
    }

    private IEnumerable<Assembly> GetRelevantAssemblies()
    {
        var loadedAssemblies = new HashSet<string>();
        var assembliesToScan = new HashSet<Assembly>();

        // Scan entry assembly and its dependencies
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            ScanAssembly(entryAssembly, loadedAssemblies, assembliesToScan);

            // Scan assemblies in the entry assembly directory
            var entryDir = Path.GetDirectoryName(entryAssembly.Location);
            if (entryDir != null)
            {
                ScanDirectory(entryDir, loadedAssemblies, assembliesToScan);
            }
        }

        // Scan currently loaded assemblies
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            ScanAssembly(assembly, loadedAssemblies, assembliesToScan);
        }

        _logger.LogInformation("Found {Count} assemblies to scan", assembliesToScan.Count);
        return assembliesToScan;
    }

    private void ScanDirectory(
        string directory,
        HashSet<string> loadedAssemblies,
        HashSet<Assembly> assembliesToScan
    )
    {
        try
        {
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);
                    if (
                        ExcludedAssemblyPrefixes.Any(prefix =>
                            assemblyName.Name?.StartsWith(
                                prefix,
                                StringComparison.OrdinalIgnoreCase
                            ) ?? false
                        )
                    )
                    {
                        continue;
                    }

                    if (!loadedAssemblies.Contains(assemblyName.Name ?? string.Empty))
                    {
                        var assembly = Assembly.Load(assemblyName);
                        ScanAssembly(assembly, loadedAssemblies, assembliesToScan);
                    }
                }
                catch (BadImageFormatException)
                {
                    // Not a .NET assembly, skip
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading assembly from file: {File}", file);
                }
            }

            // Scan subdirectories
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                ScanDirectory(subDir, loadedAssemblies, assembliesToScan);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error scanning directory: {Directory}", directory);
        }
    }

    private void ScanAssembly(
        Assembly assembly,
        HashSet<string> loadedAssemblies,
        HashSet<Assembly> assembliesToScan
    )
    {
        if (assembly.IsDynamic || assembly.Location == string.Empty)
        {
            return;
        }

        var assemblyName = assembly.GetName().Name;
        if (
            string.IsNullOrEmpty(assemblyName)
            || ExcludedAssemblyPrefixes.Any(prefix =>
                assemblyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            )
        )
        {
            return;
        }

        if (!loadedAssemblies.Add(assemblyName))
        {
            return;
        }

        assembliesToScan.Add(assembly);
        _logger.LogDebug("Added assembly to scan: {Assembly}", assembly.FullName);

        try
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            _logger.LogDebug(
                "Found {Count} references in {Assembly}",
                referencedAssemblies.Length,
                assembly.GetName().Name
            );

            foreach (var reference in referencedAssemblies)
            {
                try
                {
                    if (!loadedAssemblies.Contains(reference.Name ?? string.Empty))
                    {
                        var referencedAssembly = Assembly.Load(reference);
                        ScanAssembly(referencedAssembly, loadedAssemblies, assembliesToScan);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Could not load referenced assembly {Assembly}",
                        reference.FullName
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Error getting references for assembly {Assembly}",
                assembly.FullName
            );
        }
    }

    private IReadOnlyList<Type> GetModuleTypes(Assembly assembly)
    {
        return _moduleTypeCache.GetOrAdd(
            assembly,
            a =>
            {
                try
                {
                    _logger.LogDebug("Scanning assembly for modules: {Assembly}", a.FullName);

                    // Only look at public types that could implement IModule
                    var types = a.GetExportedTypes()
                        .Where(t =>
                            !t.IsAbstract && !t.IsInterface && typeof(IModule).IsAssignableFrom(t)
                        )
                        .ToList();

                    if (types.Any())
                    {
                        _logger.LogInformation(
                            "Found {Count} modules in {Assembly}",
                            types.Count,
                            a.GetName().Name
                        );
                    }

                    return types.AsReadOnly();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error scanning assembly {Assembly}", a.FullName);
                    return Array.Empty<Type>();
                }
            }
        );
    }
}

using System.Collections.Concurrent;
using System.Reflection;
using FastModule.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FastModule.Core.Configuration;

public class FastModuleDiscovery(ILogger logger)
{
    private readonly ConcurrentDictionary<Assembly, IReadOnlyList<Type>> _moduleTypeCache = new();

    private static readonly string[] ExcludedAssemblyPrefixes =
    [
        "System.",
        "Microsoft.",
        "Newtonsoft.",
        "Azure.",
        "WindowsBase",
        "mscorlib",
    ];

    public IEnumerable<Type> DiscoverModules()
    {
        var assemblies = GetRelevantAssemblies();
        var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();

        logger.LogInformation($"Found {0} assemblies {enumerable.Count()}");

        foreach (var assembly in enumerable)
        {
            Console.WriteLine("Assembly: {0}", assembly.FullName);
        }

        return enumerable.SelectMany(GetModuleTypes).Distinct();
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

        logger.LogInformation($"Found {assembliesToScan.Count} assemblies to scan");
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
                    logger.LogInformation("Skipping non-.NET assembly: {File}", file);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading assembly from file: {File}", file);
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
            logger.LogError(ex, "Error scanning directory: {Directory}", directory);
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
        logger.LogDebug($"Added assembly to scan: {assembly.FullName}");

        try
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            logger.LogDebug(
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
                    logger.LogWarning(
                        ex,
                        "Could not load referenced assembly {Assembly}",
                        reference.FullName
                    );
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
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
                    logger.LogDebug("Scanning assembly for modules: {Assembly}", a.FullName);

                    // Only look at public types that could implement IModule
                    var types = a.GetExportedTypes()
                        .Where(t =>
                            !t.IsAbstract
                            && !t.IsInterface
                            && typeof(IFastModule).IsAssignableFrom(t)
                        )
                        .ToList();

                    if (types.Any())
                    {
                        logger.LogInformation(
                            "Found {Count} modules in {Assembly}",
                            types.Count,
                            a.GetName().Name
                        );
                    }

                    return types.AsReadOnly();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error scanning assembly {Assembly}", a.FullName);
                    return Array.Empty<Type>();
                }
            }
        );
    }
}

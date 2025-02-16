using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FastModule.Core;

public static class ModuleManagerReflection
{
    private static readonly ConcurrentDictionary<Type, IModule> _moduleInstances = new();
    private static readonly HashSet<Type> _registeredModules = new();
    private static ILogger _logger;

    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        _logger = services.BuildServiceProvider().GetRequiredService<ILogger<IModule>>();
        var sw = Stopwatch.StartNew();

        var moduleDiscovery = new ModuleDiscovery(_logger);
        var moduleTypes = moduleDiscovery.DiscoverModules();

        _logger.LogInformation(
            "Starting module registration. Found {Count} modules to register",
            moduleTypes.Count()
        );

        foreach (var moduleType in moduleTypes)
        {
            RegisterModule(services, moduleType);
        }

        _logger.LogInformation(
            "Module registration completed in {ElapsedMs}ms. Successfully registered {Count} modules out of {TotalCount} discovered",
            sw.ElapsedMilliseconds,
            _registeredModules.Count,
            moduleTypes.Count()
        );
        return services;
    }

    private static void RegisterModule(IServiceCollection services, Type moduleType)
    {
        lock (_registeredModules)
        {
            if (!_registeredModules.Add(moduleType))
            {
                _logger.LogDebug("Module {Module} already registered, skipping registration", moduleType.Name);
                return;
            }
        }

        _logger.LogInformation("Starting registration of module: {Module}", moduleType.Name);
        var sw = Stopwatch.StartNew();

        try
        {
            var dependencies = moduleType.GetCustomAttributes<DependsOnAttribute>(false);
            var dependencyCount = dependencies.Count();
            if (dependencyCount > 0)
            {
                _logger.LogDebug("Module {Module} has {Count} dependencies", moduleType.Name, dependencyCount);
                foreach (var dependency in dependencies)
                {
                    _logger.LogDebug("Registering dependency {Dependency} for module {Module}", 
                        dependency.ModuleType.Name, moduleType.Name);
                    RegisterModule(services, dependency.ModuleType);
                }
            }

            // Lazy initialization of module instance
            var module = _moduleInstances.GetOrAdd(
                moduleType,
                t =>
                    (IModule)(
                        Activator.CreateInstance(t)
                        ?? throw new InvalidOperationException(
                            $"Failed to create instance of module {t.Name}"
                        )
                    )
            );

            module.RegisterModule(services);

            _logger.LogInformation(
                "Successfully registered module {Module} in {ElapsedMs}ms. Dependencies: {DependencyCount}",
                moduleType.Name,
                sw.ElapsedMilliseconds,
                dependencyCount
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to register module {Module}. Error: {Error}",
                moduleType.Name,
                ex.Message
            );
            throw;
        }
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation(
            "Starting endpoint mapping for {Count} registered modules",
            _registeredModules.Count
        );

        var mappedCount = 0;
        foreach (var moduleType in _registeredModules)
        {
            if (_moduleInstances.TryGetValue(moduleType, out var module))
            {
                try
                {
                    module.MapEndpoints(endpoints);
                    mappedCount++;
                    _logger.LogDebug("Successfully mapped endpoints for module {Module}", moduleType.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to map endpoints for module {Module}. Error: {Error}",
                        moduleType.Name,
                        ex.Message
                    );
                }
            }
        }

        _logger.LogInformation(
            "Endpoint mapping completed in {ElapsedMs}ms. Successfully mapped {MappedCount} out of {TotalCount} modules",
            sw.ElapsedMilliseconds,
            mappedCount,
            _registeredModules.Count
        );
        return endpoints;
    }
}

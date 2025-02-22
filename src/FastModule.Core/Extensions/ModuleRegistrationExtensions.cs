using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using FastModule.Core.Attributes;
using FastModule.Core.Configuration;
using FastModule.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FastModule.Core.Extensions;

public static class ModuleRegistrationExtensions
{
    private static readonly ConcurrentDictionary<Type, IFastModule> ModuleInstances = new();
    private static readonly ConcurrentDictionary<Type, IFastModuleEvent> moduleEventInstances = new();

    private static readonly HashSet<Type> RegisteredModules = [];
    private static ILogger _logger;

    public static HashSet<Type> GetRegisteredModules() => RegisteredModules;

    public static IServiceCollection AddFastModule(this IServiceCollection services, Action<DbContextOptionsBuilder>? options = null)
    {
        _logger = services.BuildServiceProvider().GetRequiredService<ILogger<IFastModule>>();
        var sw = Stopwatch.StartNew();

        var moduleDiscovery = new FastModuleDiscovery(_logger);
        var moduleTypes = moduleDiscovery.DiscoverModules();
        var enumerable = moduleTypes as Type[] ?? moduleTypes.ToArray();
        foreach (var moduleType in enumerable)
        {
            AddFastModule(services, moduleType, options);
        }
        _logger.LogInformation(
            "Module registration completed in {ElapsedMs}ms. Successfully registered {Count} modules out of {TotalCount} discovered",
            sw.ElapsedMilliseconds,
            RegisteredModules.Count,
            enumerable.Count()
        );
        return services;
    }

    private static void AddFastModule(this IServiceCollection services, Type moduleType, Action<DbContextOptionsBuilder>? options)
    {
        lock (RegisteredModules)
        {
            if (!RegisteredModules.Add(moduleType))
            {
                _logger.LogDebug(
                    "Module {Module} already registered, skipping registration",
                    moduleType.Name
                );
                return;
            }
        }

        _logger.LogInformation("Starting registration of module: {Module}", moduleType.Name);
        var sw = Stopwatch.StartNew();

        try
        {
            var dependencies = moduleType
                .GetCustomAttributes<DependsOnAttribute>(true)
                .Where(t => 
                    typeof(IFastModule).IsAssignableFrom(t.ModuleType) || typeof(IFastModuleEvent).IsAssignableFrom(t.ModuleType));

            var onAttributes = dependencies as DependsOnAttribute[] ?? dependencies.ToArray();
            var dependencyCount = onAttributes.Count();
            if (dependencyCount > 0)
            {
                _logger.LogDebug(
                    "Module {Module} has {Count} dependencies",
                    moduleType.Name,
                    dependencyCount
                );
                foreach (var dependency in onAttributes)
                {
                    _logger.LogDebug(
                        "Registering dependency {Dependency} for module {Module}",
                        dependency.ModuleType.Name,
                        moduleType.Name
                    );
                    AddFastModule(services, dependency.ModuleType, options);
                }
            }

            if (typeof(IFastModule).IsAssignableFrom(moduleType))
            {
                // Lazy initialization of module instance
                var module = ModuleInstances.GetOrAdd(
                    moduleType,
                    t =>
                        (IFastModule)(
                            Activator.CreateInstance(t)
                            ?? throw new InvalidOperationException(
                                $"Failed to create instance of module {t.Name}"
                            )
                        )
                );

                module.Register(services, options);
            }
        
            
            if (typeof(IFastModuleEvent).IsAssignableFrom(moduleType))
            {
                // Lazy initialization of module event instance
                var module = moduleEventInstances.GetOrAdd(
                    moduleType,
                    t =>
                        (IFastModuleEvent)(
                            Activator.CreateInstance(t)
                            ?? throw new InvalidOperationException(
                                $"Failed to create instance of module event {t.Name}"
                            )
                        )
                );

                module.RegisterEvents(services);
            }

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
}

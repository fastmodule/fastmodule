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

/// <summary>
/// Provides extension methods for registering FastModule-based modules and their dependencies into the application's dependency injection container.
/// </summary>
public static class ModuleRegistrationExtensions
{
    /// <summary>
    /// A thread-safe dictionary to store instances of registered FastModules.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IFastModule> ModuleInstances = new();

    /// <summary>
    /// A thread-safe dictionary to store instances of registered FastModule events.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IFastModuleEvent> ModuleEventInstances =
        new();

    /// <summary>
    /// A collection that keeps track of registered module types to avoid duplicate registrations.
    /// </summary>
    private static readonly HashSet<Type> RegisteredModules = [];

    /// <summary>
    /// Logger instance for logging module registration events.
    /// </summary>
    private static ILogger _logger;

    /// <summary>
    /// Retrieves the list of currently registered module types.
    /// </summary>
    /// <returns>A HashSet containing the types of registered modules.</returns>
    public static HashSet<Type> GetRegisteredModules() => RegisteredModules;

    /// <summary>
    /// Discovers and registers all available FastModules into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register modules into.</param>
    /// <param name="options">Optional action to configure database context options.</param>
    /// <returns>The updated IServiceCollection instance.</returns>
    public static IServiceCollection AddFastModule(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder>? options = null
    )
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

    /// <summary>
    /// Registers an individual FastModule and its dependencies into the service collection.
    /// </summary>
    /// <param name="services">The service collection to register the module into.</param>
    /// <param name="moduleType">The type of the module to register.</param>
    /// <param name="options">Optional action to configure database context options.</param>
    private static void AddFastModule(
        this IServiceCollection services,
        Type moduleType,
        Action<DbContextOptionsBuilder>? options
    )
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
            // Retrieve all module dependencies defined using the DependsOn attribute.
            var dependencies = moduleType
                .GetCustomAttributes<DependsOnAttribute>(true)
                .Where(t =>
                    typeof(IFastModule).IsAssignableFrom(t.ModuleType)
                    || typeof(IFastModuleEvent).IsAssignableFrom(t.ModuleType)
                );

            var onAttributes = dependencies as DependsOnAttribute[] ?? dependencies.ToArray();
            var dependencyCount = onAttributes.Length;

            if (dependencyCount > 0)
            {
                _logger.LogDebug(
                    "Module {Module} has {Count} dependencies",
                    moduleType.Name,
                    dependencyCount
                );

                // Recursively register all dependencies before registering the module itself.
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

            // Register module if it implements IFastModule.
            if (typeof(IFastModule).IsAssignableFrom(moduleType))
            {
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

            // Register module events if it implements IFastModuleEvent.
            if (typeof(IFastModuleEvent).IsAssignableFrom(moduleType))
            {
                var module = ModuleEventInstances.GetOrAdd(
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

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core;

public static class ModuleManager
{
    private static readonly List<Type> _registeredModules = new();

    public static IServiceCollection RegisterModules(
        IServiceCollection services,
        params Type[] moduleTypes
    )
    {
        foreach (var moduleType in moduleTypes)
        {
            RegisterModule(services, moduleType);
        }

        return services;
    }

    private static void RegisterModule(IServiceCollection services, Type moduleType)
    {
        if (_registeredModules.Contains(moduleType))
            return; // Module already registered

        // Ensure the type implements IModule
        if (!typeof(IModule).IsAssignableFrom(moduleType))
            throw new InvalidOperationException($"{moduleType.Name} does not implement IModule.");

        // Register dependencies first
        var dependsOnAttributes = moduleType
            .GetCustomAttributes(typeof(DependsOnAttribute), false)
            .Cast<DependsOnAttribute>();
        foreach (var attribute in dependsOnAttributes)
        {
            RegisterModule(services, attribute.ModuleType);
        }

        // Register the module itself
        var moduleInstance =
            Activator.CreateInstance(moduleType) as IModule
            ?? throw new InvalidOperationException(
                $"Failed to create instance of {moduleType.Name}"
            );
        moduleInstance.RegisterModule(services);
        _registeredModules.Add(moduleType);
    }

    public static IEndpointRouteBuilder MapModules(
        IEndpointRouteBuilder endpoints,
        params Type[] moduleTypes
    )
    {
        foreach (var moduleType in moduleTypes)
        {
            MapModule(endpoints, moduleType);
        }

        return endpoints;
    }

    private static void MapModule(IEndpointRouteBuilder endpoints, Type moduleType)
    {
        if (!_registeredModules.Contains(moduleType))
            throw new InvalidOperationException($"{moduleType.Name} has not been registered.");

        var moduleInstance =
            Activator.CreateInstance(moduleType) as IModule
            ?? throw new InvalidOperationException(
                $"Failed to create instance of {moduleType.Name}"
            );
        moduleInstance.MapEndpoints(endpoints);
    }
}

using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Routing;

namespace FastModule.Core.Extensions;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapFastModules(this IEndpointRouteBuilder endpoints)
    {
        var onlyFastModules = ModuleRegistrationExtensions.GetRegisteredModules().Where(t => typeof(IFastModule).IsAssignableFrom(t));

        foreach (var moduleType in onlyFastModules)
        {
            MapModule(endpoints, moduleType);
        }

        return endpoints;
    }

    private static void MapModule(IEndpointRouteBuilder endpoints, Type moduleType)
    {
        if (!ModuleRegistrationExtensions.GetRegisteredModules().Contains(moduleType))
            throw new InvalidOperationException($"{moduleType.Name} has not been registered.");

        var moduleInstance =
            Activator.CreateInstance(moduleType) as IFastModule
            ?? throw new InvalidOperationException(
                $"Failed to create instance of {moduleType.Name}"
            );
        Console.WriteLine($"🚀 Mapping {moduleType.Assembly.FullName} endpoints...");
        moduleInstance.AddRoutes(endpoints);
    }
}

using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Routing;

namespace FastModule.Core.Extensions;

/// <summary>
/// Provides extension methods for mapping routes for registered FastModules in the application's endpoint routing system.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all registered FastModules to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to which the FastModule routes will be added.</param>
    /// <returns>The updated <see cref="IEndpointRouteBuilder"/> with mapped FastModule routes.</returns>
    public static IEndpointRouteBuilder MapFastModules(this IEndpointRouteBuilder endpoints)
    {
        // Get all registered modules that implement IFastModule
        var onlyFastModules = ModuleRegistrationExtensions.GetRegisteredModules()
            .Where(t => typeof(IFastModule).IsAssignableFrom(t));

        // Iterate over each registered module and map its routes
        foreach (var moduleType in onlyFastModules)
        {
            MapModule(endpoints, moduleType);
        }

        return endpoints;
    }

    /// <summary>
    /// Maps the routes for a specific FastModule if it has been registered.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to add the module's routes to.</param>
    /// <param name="moduleType">The type of the module whose routes are to be mapped.</param>
    /// <exception cref="InvalidOperationException">Thrown if the module has not been registered or fails to create an instance.</exception>
    private static void MapModule(IEndpointRouteBuilder endpoints, Type moduleType)
    {
        // Ensure that the module has been registered before attempting to map it
        if (!ModuleRegistrationExtensions.GetRegisteredModules().Contains(moduleType))
        {
            throw new InvalidOperationException($"{moduleType.Name} has not been registered.");
        }

        // Create an instance of the module
        var moduleInstance = Activator.CreateInstance(moduleType) as IFastModule
            ?? throw new InvalidOperationException($"Failed to create instance of {moduleType.Name}");

        // Log the mapping process
        Console.WriteLine($"🚀 Mapping {moduleType.Assembly.FullName} endpoints...");

        // Add the module's routes to the endpoint route builder
        moduleInstance.AddRoutes(endpoints);
    }
}
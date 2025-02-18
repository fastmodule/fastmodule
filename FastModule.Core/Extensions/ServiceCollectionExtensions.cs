using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Extensions;

public static class ServiceCollectionExtensions
{

    public static void RegisterTransients(this IServiceCollection services, Assembly assembly)
    {
        // Get all types from the assembly
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            // Find interfaces that the type implements
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                // Register the type as a transient service for each interface it implements
                if (@interface.Name == $"I{type.Name}")
                {
                    services.AddTransient(@interface, type);
                }
            }
        }
    }
    
    public static void RegisterScopes(this IServiceCollection services, Assembly assembly)
    {
        // Get all types from the assembly
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            // Find interfaces that the type implements
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                // Register the type as a scoped service for each interface it implements
                if (@interface.Name == $"I{type.Name}")
                {
                    services.AddScoped(@interface, type);
                }
            }
        }
    }
    
    public static void RegisterSingletons(this IServiceCollection services, Assembly assembly)
    {
        // Get all types from the assembly
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            // Find interfaces that the type implements
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                // Register the type as a scoped service for each interface it implements
                if (@interface.Name == $"I{type.Name}")
                {
                    services.AddSingleton(@interface, type);
                }
            }
        }
    }
}

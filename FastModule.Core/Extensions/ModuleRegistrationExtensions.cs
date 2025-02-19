using System.Reflection;
using FastModule.Core.Attributes;
using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FastModule.Core.Extensions;

public static class ModuleRegistrationExtensions
{
    public static void RegisterModules(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IFastModule).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .ToList();

        if (!moduleTypes.Any())
        {
            Console.WriteLine("❌ No modules found for registration!");
            return;
        }

        Console.WriteLine($"🔹 Discovered {moduleTypes.Count} modules...");
        

        foreach (var moduleType in moduleTypes)
        {
            var dependsOnAttributes = moduleType.GetCustomAttributes(typeof(DependsOnAttribute), true)
                .Cast<DependsOnAttribute>()
                .ToList();
            
            foreach (var dependsOn in dependsOnAttributes)
            {
                Console.WriteLine($"🔹 Module depends on {dependsOn.ModuleType}...");
                Console.WriteLine($"🔹 Module name is {moduleType.Name}");
                var moduleInstance = (IFastModule)Activator.CreateInstance(dependsOn.ModuleType)!;
                moduleInstance.Register(services, configuration);

                // ✅ Register API modules implementing IEndpointDefinition
                if (typeof(IEndpointDefinition).IsAssignableFrom(moduleType))
                {
                    services.AddSingleton(typeof(IEndpointDefinition), moduleInstance);
                    Console.WriteLine($"✅ Registered API Module: {moduleType.Name}");
                }
            }
            
            
         
        }
    }
    
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpointDefinition)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpointDefinition), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }
    
    
    public static IApplicationBuilder MapFastModuleEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpointDefinition>>();

        IEndpointRouteBuilder builder =
            routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}


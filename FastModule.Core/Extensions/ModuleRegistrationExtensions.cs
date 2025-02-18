using FastModule.Core.Attributes;
using FastModule.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Extensions;

public static class ModuleRegistrationExtensions
{
    public static void RegisterModules(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        // Create a dictionary to track dependencies
        var moduleDependencyMap = new Dictionary<Type, List<Type>>();

        //  Ensure all modules exist in the dictionary first to avoid null reference exceptions
        foreach (var module in moduleTypes)
        {
            if (!moduleDependencyMap.ContainsKey(module))
            {
                moduleDependencyMap[module] = new List<Type>(); // Initialize empty dependency list
            }
        }

        // Populate dependencies
        foreach (var module in moduleTypes)
        {
            var dependsOnAttributes = module.GetCustomAttributes(typeof(DependsOnAttribute), true)
                .Cast<DependsOnAttribute>();

            var dependencies = dependsOnAttributes.SelectMany(attr => attr.Dependencies).ToList();

            // Add dependencies to existing module entry
            if (moduleDependencyMap.ContainsKey(module))
            {
                moduleDependencyMap[module].AddRange(dependencies);
            }
        }

        // Sort modules based on dependencies
        var sortedModules = TopologicalSort(moduleDependencyMap);

        // Register modules in the correct order
        foreach (var moduleType in sortedModules)
        {
            var moduleInstance = (IModule)Activator.CreateInstance(moduleType);
            moduleInstance.Register(services, configuration);
        }
    }
    
    private static List<Type> TopologicalSort(Dictionary<Type, List<Type>> dependencyMap)
    {
        var sorted = new List<Type>();
        var visited = new HashSet<Type>();

        void Visit(Type module)
        {
            if (visited.Contains(module))
                return;

            if (!dependencyMap.ContainsKey(module))
            {
                throw new KeyNotFoundException($"Module {module.FullName} is not present in the dependency map.");
            }

            visited.Add(module);

            foreach (var dependency in dependencyMap[module])
            {
                Visit(dependency);
            }

            sorted.Add(module);
        }

        Console.WriteLine("Discovered Modules:");
        foreach (var module in dependencyMap.Keys)
        {
            Console.WriteLine($" - {module.FullName}");
            Visit(module);
        }

        return sorted;
    }

}

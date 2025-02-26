using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core;

/// <summary>
/// Represents a base module that can be registered in the application's dependency injection container
/// and provides functionality for configuring database context, routing, and event registration.
/// </summary>
public class FastModule : IFastModule, IFastModuleEvent
{
    /// <summary>
    /// Registers services required by the module.
    /// </summary>
    /// <param name="services">The service collection to register dependencies.</param>
    /// <param name="options">Optional action to configure database context options.</param>
    public virtual void Register(
        IServiceCollection services,
        Action<DbContextOptionsBuilder>? options = null
    )
    {
        // Implementation for registering module-specific services
    }

    /// <summary>
    /// Adds API routes for the module.
    /// </summary>
    /// <param name="app">The endpoint route builder to define routes.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public virtual IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        return app;
    }

    /// <summary>
    /// Registers event handlers or background services required by the module.
    /// </summary>
    /// <param name="services">The service collection to register event-related services.</param>
    public virtual void RegisterEvents(IServiceCollection services)
    {
        // Implementation for registering module-specific events
    }
}

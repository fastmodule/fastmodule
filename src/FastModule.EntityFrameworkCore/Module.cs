using FastModule.EntityFrameworkCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.EntityFrameworkCore;

/// <summary>
/// Represents a module that registers database-related services.
/// This module is responsible for configuring and registering the <see cref="AppDbContext"/> with the dependency injection container.
/// </summary>
public sealed class Module : Core.FastModule
{
    /// <summary>
    /// Registers the <see cref="AppDbContext"/> with the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the DbContext should be added.</param>
    /// <param name="options">An optional action to configure the <see cref="DbContextOptionsBuilder"/>.</param>
    public override void Register(
        IServiceCollection services,
        Action<DbContextOptionsBuilder>? options = null
    )
    {
        // If no options are provided, do not register the DbContext
        if (options is null)
            return;

        // Register the AppDbContext with the provided configuration options
        services.AddDbContext<AppDbContext>(options);
    }
}

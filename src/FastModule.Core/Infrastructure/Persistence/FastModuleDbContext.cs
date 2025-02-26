using Microsoft.EntityFrameworkCore;

namespace FastModule.Core.Infrastructure.Persistence;

/// <summary>
/// A generic abstract base class for database contexts used in FastModule.
/// Provides common configuration and model-building behavior for derived DbContext classes.
/// </summary>
/// <typeparam name="TDbContext">The specific DbContext type that inherits from this class.</typeparam>
/// <param name="options">The database context options for configuration.</param>
public abstract class FastModuleDbContext<TDbContext>(DbContextOptions<TDbContext> options)
    : DbContext(options)
    where TDbContext : DbContext
{
    /// <summary>
    /// Stores the database context options for further use in derived classes.
    /// </summary>
    protected DbContextOptions DbContextOptions = options;

    /// <summary>
    /// Configures the database context before it is initialized.
    /// Allows additional configuration in derived classes.
    /// </summary>
    /// <param name="optionsBuilder">The options builder for configuring the database context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Configures the entity models and relationships for the database context.
    /// Can be overridden in derived classes to customize model creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to define entity relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

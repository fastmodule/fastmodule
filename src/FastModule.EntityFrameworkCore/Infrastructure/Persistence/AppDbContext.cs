using FastModule.Core.Domain.Constants;
using FastModule.Core.Infrastructure.Persistence;
using FastModule.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace FastModule.EntityFrameworkCore.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context, responsible for interacting with the database.
/// Inherits from <see cref="FastModuleDbContext{TDbContext}"/> to support modular database management.
/// </summary>
/// <param name="options">The database context options, including the connection string and provider settings.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : FastModuleDbContext<AppDbContext>(options)
{
    /// <summary>
    /// Gets or sets the database table for managing users.
    /// </summary>
    public DbSet<UserEntity> Users { get; set; } = null!;

    /// <summary>
    /// Configures the entity relationships and constraints for the database model.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the UserEntity table name with a prefix
        modelBuilder.Entity<UserEntity>().ToTable($"{TablePrefix.Prefix}users");

        // Defines the entity configuration for UserEntity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id); // Sets primary key
            entity.Property(e => e.Email).IsRequired(); // Requires Email field
            entity.Property(e => e.SubjectId).IsRequired(); // Requires SubjectId field
        });
    }
}

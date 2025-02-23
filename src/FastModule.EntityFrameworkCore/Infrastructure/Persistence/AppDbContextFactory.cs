using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FastModule.EntityFrameworkCore.Infrastructure.Persistence;

/// <summary>
/// Factory for creating instances of <see cref="AppDbContext"/> at design time.
/// This is required for running EF Core migrations without needing a running application.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of <see cref="AppDbContext"/> using a database connection string.
    /// This method is invoked by the Entity Framework Core CLI commands such as migrations.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    /// <returns>A new instance of <see cref="AppDbContext"/>.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build the application configuration
        var config = BuildConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Retrieve the database connection string from configuration
        var connectionString = config.GetConnectionString("DefaultConnection");

        // Configure PostgreSQL as the database provider and set migration settings
        optionsBuilder.UseNpgsql(connectionString, x => 
            x.MigrationsHistoryTable("fast_module_migrations") // Set custom migrations history table
             .MigrationsAssembly("FastModule.Migrator")); // Specify the assembly containing migrations

        return new AppDbContext(optionsBuilder.Options);
    }
    
    /// <summary>
    /// Builds the application configuration by loading settings from JSON files and environment variables.
    /// </summary>
    /// <returns>An <see cref="IConfigurationRoot"/> containing the application settings.</returns>
    private static IConfigurationRoot BuildConfiguration()
    {
        // Determine the current environment (defaults to "Development" if not set)
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Configure the settings file paths
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FastModule.Migrator/")) // Set base path to Migrator project
            .AddJsonFile("appsettings.json", optional: false) // Load default settings file
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) // Load environment-specific settings if available
            .AddEnvironmentVariables(); // Include environment variables

        return builder.Build();
    }
}

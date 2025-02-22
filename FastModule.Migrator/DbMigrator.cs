using FastModule.EntityFrameworkCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FastModule.Migrator;

public class DbMigrator(IServiceProvider serviceProvider, ILogger<DbMigrator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        logger.LogInformation("Migrating the database...");
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        logger.LogInformation("Database migration completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
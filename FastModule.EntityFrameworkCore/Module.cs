using FastModule.EntityFrameworkCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FastModule.EntityFrameworkCore;

public sealed class Module : Core.FastModule
{
    public override void Register(IServiceCollection services, Action<DbContextOptionsBuilder>? options = null)
    {
        if (options is not null)
        {
            services.AddDbContext<AppDbContext>(options);
            ApplyMigrations(services.BuildServiceProvider()).Wait();
        }
       
    }
    
    private async Task ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (!env.IsDevelopment())
        {
            return;
        }
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();
    }
}

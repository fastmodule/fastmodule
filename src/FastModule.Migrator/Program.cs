using FastModule.Core.Extensions;
using FastModule.Migrator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(
                (ctx, configuration) =>
                {
                    configuration.Sources.Clear();

                    configuration
                        .SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env}.json", true, true)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables();
                }
            )
            .ConfigureServices(
                (ctx, services) =>
                {
                    services.AddFastModule(
                        (options) =>
                        {
                            options.UseNpgsql(
                                ctx.Configuration.GetConnectionString("DefaultConnection"),
                                x =>
                                    x.MigrationsHistoryTable("fast_module_migrations")
                                        .MigrationsAssembly("FastModule.Migrator")
                            );
                        }
                    );
                    services.AddHostedService<DbMigrator>();
                }
            );

        host.UseEnvironment(env);
        return host;
    }
}

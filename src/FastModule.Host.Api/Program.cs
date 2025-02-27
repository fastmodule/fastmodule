using FastModule.Core.Extensions;
using FastModule.Host.Api.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastModule(
    (options) =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.MigrationsHistoryTable("fast_module_migrations")
        );
    }
);

var app = builder.Build();
app.ConfigureDevelopmentEnvironment();

// Configure middleware
app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
    }
);

app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.MapFastModules();

app.Run();

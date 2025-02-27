using FastModule.Core.Attributes;
using FastModule.Host.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace FastModule.Host.Api;

[DependsOn(typeof(Keycloak.Module))]
[DependsOn(typeof(User.Module))]
public sealed class ApiHostModule : Core.FastModule
{
    public override void Register(
        IServiceCollection services,
        Action<DbContextOptionsBuilder>? options = null
    )
    {
        Console.WriteLine("✅ Bootstrap ApiHostModule Registered in DI.");
    }

    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var host = app.MapGroup("/host").WithTags("host");
        new Test().MapEndpoint(host);
        return app;
    }
}

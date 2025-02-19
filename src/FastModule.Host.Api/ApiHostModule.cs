using FastModule.Core.Attributes;
using FastModule.Core.Interfaces;
using FastModule.Host.Api.Endpoints;

namespace FastModule.Host.Api;

[DependsOn(typeof(Keycloak.Module))]
[DependsOn(typeof(User.Module))]
public class ApiHostModule : IFastModule
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine("✅ Bootstrap ApiHostModule Registered in DI.");
    }

    public IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var host = app.MapGroup("/host").WithTags("host");
        new Test().MapEndpoint(host);
        return app;
    }
}

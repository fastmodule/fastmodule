using FastModule.Core.Interfaces;

namespace FastModule.Host.Api.Endpoints;

public sealed class Test : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/test", () => Results.Ok("Hello, World!"));
        return app;
    }
}

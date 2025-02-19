using System.Net;
using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FastModule.Keycloak.Endpoints;

public class Webhook : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook", ProcessWebhook);
        return app;
    }

    private IResult ProcessWebhook(HttpContext ctx, dynamic Payload)
    {
        return Results.Ok(HttpStatusCode.Accepted);
    }
}

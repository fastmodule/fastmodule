using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using FastModule.Core.Interfaces;
using FastModule.Shared.Events;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FastModule.Keycloak.Endpoints;

public class Webhook(IMediator mediator) : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/webhook", ProcessWebhook);
        return app;
    }

    private async Task<IResult> ProcessWebhook(HttpContext ctx, object payload)
    {

        // Todo: Implement validation of the payload based of the event that is triggered from Keycloak. 
        string jsonPayload = payload.ToString();  

        // Parse the JSON string to a JsonDocument
        var doc = JsonDocument.Parse(jsonPayload);
        var root = doc.RootElement;

        var newUserCreatedEvent = new NewUserCreatedEvent
        {
            FullName = root.GetProperty("fullName").ToString(),
            Email = root.GetProperty("email").ToString(),
            Username = root.GetProperty("userName").ToString(),
            SubjectId = root.GetProperty("sub").ToString()
        };
        await mediator.Publish(newUserCreatedEvent);
        return Results.Ok(HttpStatusCode.Accepted);
    }
}

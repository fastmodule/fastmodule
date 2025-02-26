using FastModule.Core.Interfaces;
using Keycloak.AuthServices.Sdk.Admin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak.Endpoints;

public sealed class KeycloakAdmin(IHttpContextAccessor contextAccessor) : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        var isAdminEnabled = app.ServiceProvider.GetService<IKeycloakClient>() != null;
        if (!isAdminEnabled)
        {
            return app;
        }
        var keycloakClient = app.ServiceProvider.GetRequiredService<IKeycloakClient>();
        var keycloak = app.MapGroup("/realm").RequireAuthorization("admin");
        keycloak.MapGet(
            "/",
            async () =>
            {
                var user = await keycloakClient.GetRealmAsync("sajantest");
                return Results.Ok(user);
            }
        );
        return app;
    }
}

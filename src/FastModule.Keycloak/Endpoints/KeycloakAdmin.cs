using System.ComponentModel;
using FastModule.Core.Interfaces;
using FastModule.Keycloak.Configurations;
using FastModule.Shared.Dtos;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.AuthServices.Sdk.Admin.Requests.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak.Endpoints;

public sealed class KeycloakAdmin(IHttpContextAccessor contextAccessor, KeycloakSetting keycloakSetting)
    : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        var keycloakClient = app.ServiceProvider.GetRequiredService<IKeycloakClient>();
        var realm = keycloakSetting.Realm;
        var keycloak = app.MapGroup("/realm").RequireAuthorization("admin");

        keycloak.MapGet(
            "/users",
            async (int? page, int? limit, [Description("Search for a string contained in Username, FirstName, LastName or Email.")] string? search) =>
            {
                var query = new GetUsersRequestParameters
                {
                    BriefRepresentation = true,
                    First = page ?? 0,
                    Max = limit ?? 10,
                    Search = search,
                };

                var totalUserCount = await keycloakClient
                    .GetUserCountAsync(realm, new GetUserCountRequestParameters
                    {
                        Search = search,
                    });

                var users = await keycloakClient.GetUsersAsync(realm, query);
                var paginatedResponse = new PaginatedResponse<UserRepresentation>
                {
                    Items = users.ToList(),
                    TotalCount = totalUserCount,
                    CurrentPage = page ?? 0,
                    PageSize = limit ?? 10,
                };
                return Results.Ok(paginatedResponse);
            }
        ).Produces<PaginatedResponse<UserRepresentation>>()
            .WithDescription("Get all users in the realm with pagination support.");
        return app;
    }
}
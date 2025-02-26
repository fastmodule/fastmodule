using FastModule.Keycloak.Configurations;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak.Extensions;

using Microsoft.OpenApi.Models;

public static class OAuthExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        KeycloakSetting keycloakSetting
    )
    {
        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "OAuth2",
                    },
                },
                ["openid", "profile", "email", "api", "offline_access"]
            },
        };

        var securityDefinitions = new Dictionary<string, OpenApiSecurityScheme>
        {
            {
                "OAuth2",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(
                                $"{keycloakSetting.Authority}/protocol/openid-connect/auth"
                            ),
                            TokenUrl = new Uri(
                                $"{keycloakSetting.Authority}/protocol/openid-connect/token"
                            ),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect" },
                                { "profile", "Access profile information" },
                                { "email", "Access email information" },
                                { "api", "Access API" },
                                { "offline_access", "Offline access" },
                            },
                        },
                    },
                }
            },
        };

        services.AddKeycloakWebApiAuthentication(config =>
        {
            config.Audience = keycloakSetting.Audience;
            config.AuthServerUrl = keycloakSetting.BaseUrl;
            config.Realm = keycloakSetting.Realm;
            config.DisableRolesAccessTokenMapping = false;
            config.VerifyTokenAudience = true;
        });

        services.AddKeycloakAuthorization(option =>
        {
            option.AuthServerUrl = keycloakSetting.BaseUrl;
            option.Realm = keycloakSetting.Realm;
            option.Resource = keycloakSetting.ClientId;
        });

        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                "admin",
                builder =>
                {
                    builder.RequireResourceRoles(["admin-client-role", "manage-users"]);
                }
            );

        services.AddOpenApi(options =>
        {
            options.AddOperationTransformer(
                (operation, context, arg3) =>
                {
                    operation.Security = new List<OpenApiSecurityRequirement>
                    {
                        securityRequirement,
                    };
                    return Task.CompletedTask;
                }
            );

            options.AddDocumentTransformer(
                (document, context, arg3) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = securityDefinitions;
                    return Task.CompletedTask;
                }
            );
        });
        return services;
    }
}

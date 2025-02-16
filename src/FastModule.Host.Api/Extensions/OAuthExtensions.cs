using FastModule.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FastModule.Host.Api.Extensions;

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

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakSetting.Authority;
                options.Audience = keycloakSetting.Audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });

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

        services.AddAuthorization();
        return services;
    }
}

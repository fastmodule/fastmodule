using FastModule.Keycloak.Configurations;
using Scalar.AspNetCore;

namespace FastModule.Host.Api.Extensions;

public static class DevelopmentExtensions
{
    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        // Development-specific configuration
        if (!app.Environment.IsDevelopment())
            return app;

        var keycloakSetting =
            app.Configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>()
            ?? throw new InvalidOperationException("KeycloakSetting is not configured");

        app.MapScalarApiReference(config =>
        {
            config
                .WithPreferredScheme("OAuth2")
                .WithTheme(ScalarTheme.Mars)
                .WithOAuth2Authentication(o =>
                {
                    o.Scopes = ["openid", "profile", "email", "api", "offline_access"];
                    o.ClientId = keycloakSetting.ClientId;
                });
        });
        app.MapOpenApi();
        return app;
    }
}

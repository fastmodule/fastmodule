using FastModule.Core.Configuration;
using FastModule.Core.Interfaces;
using FastModule.Host.Api.Extensions;
using Scalar.AspNetCore;

namespace FastModule.Host.Api;


public class ApiModule() : IModule, IFastApplicationModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Register API-specific services
        services.AddControllers().AddApplicationPart(typeof(ApiModule).Assembly);
        var keycloakSetting = configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>()
                              ?? throw new InvalidOperationException("KeycloakSetting is not configured");
        services.AddKeycloakAuthentication(keycloakSetting);
    }

    public async Task InitiateAsync(WebApplication app)
    {
        
        // Development-specific configuration
        if (app.Environment.IsDevelopment())
        {
            var keycloakSetting = app.Configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>();

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
        }

        // Configure middleware
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers()
            .RequireAuthorization();
        
        await Task.CompletedTask;
    }
}

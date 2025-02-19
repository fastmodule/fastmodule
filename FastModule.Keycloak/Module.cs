using FastModule.Core.Configuration;
using FastModule.Core.Interfaces;
using FastModule.Keycloak.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak;

public class Module : IFastModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("✅ KeycloakModule Registered in DI.");
        services.AddHttpContextAccessor();
        services.AddAuthorization();
        var keycloakSetting = configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>()
                              ?? throw new InvalidOperationException("KeycloakSetting is not configured");
        services.AddKeycloakAuthentication(keycloakSetting);
    }
}

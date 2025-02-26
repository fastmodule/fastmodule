using FastModule.Keycloak.Configurations;
using Keycloak.AuthServices.Common;
using Keycloak.AuthServices.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak.Extensions;

public static class KeycloakAdminApiExtensions
{
    public static IServiceCollection AddKeycloakAdminApi(
        this IServiceCollection services,
        KeycloakSetting keycloakSetting
    )
    {
        services.AddDistributedMemoryCache();

        services
            .AddClientCredentialsTokenManagement()
            .AddClient(
                "admin-api",
                client =>
                {
                    client.ClientId = keycloakSetting.AdminApi!.ClientId;
                    client.ClientSecret = keycloakSetting.AdminApi!.ClientSecret;
                    client.TokenEndpoint =
                        $"{keycloakSetting.Authority}/protocol/openid-connect/token";
                }
            );

        services
            .AddKeycloakAdminHttpClient(config =>
            {
                config.Resource = keycloakSetting.ClientId;
                config.Credentials = new KeycloakClientInstallationCredentials()
                {
                    Secret = keycloakSetting.AdminApi!.ClientSecret,
                };
                config.AuthServerUrl = keycloakSetting.BaseUrl;
                config.Realm = keycloakSetting.Realm;
                config.VerifyTokenAudience = true;
            })
            .AddClientCredentialsTokenHandler("admin-api");
        return services;
    }
}

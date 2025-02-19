using FastModule.Core.Configuration;
using FastModule.Core.Extensions;
using FastModule.Core.Interfaces;
using FastModule.Keycloak.Endpoints;
using FastModule.Keycloak.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Keycloak;

public class Module : IFastModule
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine("✅ KeycloakModule Registered in DI.");
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var keycloakSetting =
            configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>()
            ?? throw new InvalidOperationException("KeycloakSetting is not configured");
        services
            .AddKeycloakAuthentication(keycloakSetting)
            .AddAuthorization()
            .AddHttpContextAccessor();
    }

    public IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var keycloak = app.MapGroup("/keycloak").WithTags("hooks");
        new Webhook().MapEndpoint(keycloak);
        return keycloak;
    }
}

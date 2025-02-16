using System.Security.Claims;
using FastModule.Core;
using FastModule.Core.Configuration;
using FastModule.Host.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Add services
builder.Services.RegisterModules();

var keycloakSetting =
    builder.Configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>()
    ?? throw new InvalidOperationException("KeycloakSetting is not configured");
builder.Services.AddKeycloakAuthentication(keycloakSetting);

var app = builder.Build();

// Development-specific configuration
if (app.Environment.IsDevelopment())
{
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
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// API endpoints
app.MapGet(
        "/user",
        (ClaimsPrincipal user) => Results.Ok(user.Claims.ToDictionary(c => c.Type, c => c.Value))
    )
    .RequireAuthorization()
    .WithName("GetUserClaims")
    .WithDescription("Returns the claims for the current user");

app.MapModules();
app.Run();

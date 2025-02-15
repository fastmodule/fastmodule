using System.Security.Claims;
using FastModule.Core.Configuration;
using FastModule.Host.Api.Extensions;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

var keycloakSetting = builder.Configuration.GetSection("KeycloakSetting").Get<KeycloakSetting>();
builder.Services.AddKeycloakAuthentication(keycloakSetting);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(config =>
    {
        config.WithPreferredScheme("OAuth2")
            .WithTheme(ScalarTheme.Mars)
            .WithOAuth2Authentication(o =>
            {
                o.Scopes = ["openid", "profile", "email", "api", "offline_access"];
                o.ClientId = keycloakSetting.ClientId;
            });
    });
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();


app.MapGet("/user", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();

app.Run();


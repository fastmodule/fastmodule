using FastModule.Core.Extensions;
using FastModule.Host.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastModule();

var app = builder.Build();
app.ConfigureDevelopmentEnvironment();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.MapFastModules();

app.Run();

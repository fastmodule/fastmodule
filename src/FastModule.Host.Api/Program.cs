using FastModule.Core.Extensions;
using FastModule.Host.Api.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Register modules
builder.Services.RegisterModules(builder.Configuration);

var app = builder.Build();
app.ConfigureDevelopmentEnvironment();


// Configure middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.MapFastModuleEndpoints(app.MapGroup("/api").RequireAuthorization());
app.Run();

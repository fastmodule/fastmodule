
using FastModule.Core.Extensions;
using FastModule.Host.Api;


var builder = WebApplication.CreateBuilder(args);

// Register all discovered modules
builder.Services.RegisterModules(builder.Configuration);
var app = builder.Build();

// Create an instance of ApiModule and initialize it

var apiModule = new ApiModule();
await apiModule.InitiateAsync(app);

app.Run();

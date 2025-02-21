using FastModule.Core.Interfaces;
using FastModule.User.Endpoints;
using FastModule.User.Interfaces;
using FastModule.User.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.User;

public class Module : IFastModule, IFastModuleEvent
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine("✅ UserModule Registered in DI.");
        services.AddTransient<IUserService, UserService>();

    }

    public IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var usersApi = app.MapGroup("/users").WithTags("users").RequireAuthorization();
        new GetUsers().MapEndpoint(usersApi);
        new UserMe().MapEndpoint(usersApi);
        return app;
    }

    public void RegisterEvents(IServiceCollection services)
    {
        Console.WriteLine("✅ UserModule Events Registered in DI.");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Module>());

    }
}

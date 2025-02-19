
using FastModule.Core.Attributes;
using FastModule.Core.Extensions;
using FastModule.Core.Interfaces;
using FastModule.Shared.Events;
using FastModule.User.Endpoints;
using FastModule.User.Interfaces;
using FastModule.User.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.User;

[DependsOn(typeof(Shared.Module))]
public class Module : IFastModule
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine("✅ UserModule Registered in DI.");
        services.AddTransient<IUserService, UserService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Module>());

    }

    public IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var usersApi = app.MapGroup("/users").WithTags("users").RequireAuthorization();
        new GetUsers().MapEndpoint(usersApi);
        new UserMe().MapEndpoint(usersApi);
        return app;
    }
}

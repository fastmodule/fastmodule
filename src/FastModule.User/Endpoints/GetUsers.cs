using FastModule.User.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.User.Endpoints;

using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public sealed class GetUsers : IEndpointDefinition
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        var scope = app.ServiceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        app.MapGet(
            "/",
            async () =>
            {
                var users = await userService.GetUsers();
                return Results.Ok(users);
            }
        );

        Console.WriteLine("✅ Endpoints registered for UserModule!");
        return app;
    }
}

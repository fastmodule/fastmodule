namespace FastModule.User.Endpoints;

using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


public sealed class GetUsers : IEndpointDefinition
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var usersApi = app
            .MapGroup("/users")
            .WithTags("users");
        
        usersApi.MapGet("/", () =>
        {
            var users = new[]
            {
                new { Id = 1, Name = "Alice" },
                new { Id = 2, Name = "Bob" },
                new { Id = 3, Name = "Charlie" },
            };
            return Results.Ok(users);
        });

        Console.WriteLine("✅ Endpoints registered for UserModule!");
    }
}

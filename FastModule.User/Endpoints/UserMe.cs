using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FastModule.User.Endpoints;

public class UserMe : IEndpointDefinition
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var usersApi = app
            .MapGroup("/users")
            .WithTags("users");
        
        usersApi.MapGet("/me", (HttpContext context) =>
        {
            var user = context.User;
            var myClaims = user?.Claims.ToDictionary(c => c.Type, c => c.Value);
            return Results.Ok(myClaims);
        });
    }
}
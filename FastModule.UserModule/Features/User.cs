using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastModule.UserModule.Features;

[ApiController]
[Route("api/users")]
[Tags("users")]
public sealed class User(IHttpContextAccessor contextAccessor) : Controller
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var user = contextAccessor.HttpContext?.User;
        var myClaims = user?.Claims.ToDictionary(c => c.Type, c => c.Value);
        return Ok(myClaims);
    }
    
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(
            new[]
            {
                new { Id = 1, Name = "Alice" },
                new { Id = 2, Name = "Bob" },
                new { Id = 3, Name = "Charlie" },
            }
        );
    }
}

using Microsoft.AspNetCore.Routing;

namespace FastModule.Core.Interfaces;

public interface IEndpointDefinition
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder app);
}

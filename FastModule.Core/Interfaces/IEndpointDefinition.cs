using Microsoft.AspNetCore.Routing;

namespace FastModule.Core.Interfaces;

public interface IEndpointDefinition
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

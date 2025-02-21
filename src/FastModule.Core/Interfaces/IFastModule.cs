using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Interfaces;

public interface IFastModule
{
    void Register(IServiceCollection services);

    IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app);
}

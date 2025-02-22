using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Interfaces;

public interface IFastModule
{
    void Register(IServiceCollection services, Action<DbContextOptionsBuilder>? options = null);

    IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app);
}

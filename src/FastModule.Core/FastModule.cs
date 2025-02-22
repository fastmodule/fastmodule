using FastModule.Core.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core;

public class FastModule : IFastModule, IFastModuleEvent
{
    public virtual void Register(IServiceCollection services, Action<DbContextOptionsBuilder>? options = null)
    {
        
    }

    public virtual IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        return app;
    }

    public virtual void RegisterEvents(IServiceCollection services)
    {
        
    }
}
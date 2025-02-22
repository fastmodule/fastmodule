using FastModule.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Shared;

public sealed class Module : Core.FastModule
{
    public override void RegisterEvents(IServiceCollection services)
    {
        base.RegisterEvents(services);
    }
}
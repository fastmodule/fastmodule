using FastModule.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Shared;

public sealed class Module : IFastModuleEvent
{
    public void RegisterEvents(IServiceCollection services)
    {
        Console.WriteLine("✅ SharedModule Registered in DI.");

    }
}
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Interfaces;

public interface IFastModuleEvent
{
    void RegisterEvents(IServiceCollection services);
}

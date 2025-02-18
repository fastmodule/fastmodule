using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Interfaces;

public interface IModule
{
    void Register(IServiceCollection services, IConfiguration configuration);
}

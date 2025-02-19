using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.Core.Interfaces;

public interface IFastModule
{
    void Register(IServiceCollection services, IConfiguration configuration);
}

using FastModule.Core.Attributes;
using FastModule.Core.Extensions;
using FastModule.Core.Interfaces;
using FastModule.User.Interfaces;
using FastModule.User.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.User;


[DependsOn(typeof(Keycloak.Module))]
public class Module : IFastModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("✅ UserModule Registered in DI.");

        services.AddTransient<IUserService, UserService>();
        
        services.AddEndpoints(typeof(Module).Assembly);
    }
}

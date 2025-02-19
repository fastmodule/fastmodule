using FastModule.Core.Attributes;
using FastModule.Core.Interfaces;

namespace FastModule.Host.Api;

[DependsOn(typeof(Keycloak.Module))]
[DependsOn(typeof(User.Module))]
public class ApiHostModule : IFastModule
{
    
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine("✅ Bootstrap ApiHostModule Registered in DI.");
    }
}


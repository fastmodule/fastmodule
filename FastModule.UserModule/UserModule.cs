using System.Reflection;
using FastModule.Core.Extensions;
using FastModule.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastModule.UserModule;

public sealed class UserModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterScopes(Assembly.GetExecutingAssembly());
        services.AddControllers().AddApplicationPart(typeof(UserModule).Assembly);
    }
}

using FastModule.EntityFrameworkCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FastModule.EntityFrameworkCore;

public sealed class Module : Core.FastModule
{
    public override void Register(IServiceCollection services, Action<DbContextOptionsBuilder>? options = null)
    {
        if (options is null) return;
        services.AddDbContext<AppDbContext>(options);
    }
}

using Microsoft.AspNetCore.Builder;

namespace FastModule.Core.Interfaces;

public interface IFastApplicationModule
{
    Task InitiateAsync(WebApplication app);
}
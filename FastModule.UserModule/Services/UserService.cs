using FastModule.UserModule.Interfaces;

namespace FastModule.UserModule.Services;

public sealed class UserService : IUserService
{
    public string GetUserName()
    {
       return "Sajan";
    }
}

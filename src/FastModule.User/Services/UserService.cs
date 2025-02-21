using FastModule.User.Interfaces;

namespace FastModule.User.Services;

public sealed class UserService : IUserService
{
    public string GetUserName()
    {
        return "Sajan";
    }
}

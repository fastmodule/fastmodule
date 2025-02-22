using FastModule.Domain.Entities.User;

namespace FastModule.User.Interfaces;

public interface IUserService
{
    Task<List<UserEntity>> GetUsers();
}

using FastModule.Domain.Entities.User;
using FastModule.EntityFrameworkCore.Infrastructure.Persistence;
using FastModule.User.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastModule.User.Services;

public sealed class UserService(AppDbContext dbContext) : IUserService
{
    public async Task<List<UserEntity>> GetUsers()
    {
        return await dbContext.Users.AsNoTracking().ToListAsync();
    }
}

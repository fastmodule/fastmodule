using Microsoft.EntityFrameworkCore;

namespace FastModule.Host.Api.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
}

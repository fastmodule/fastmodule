using Microsoft.EntityFrameworkCore;

namespace FastModule.Core.Infrastructure.Persistence;

public abstract class FastModuleDbContext<TDbContext>(DbContextOptions<TDbContext> options) : DbContext(options)
    where TDbContext : DbContext
{
    protected DbContextOptions dbContextOptions = options;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}
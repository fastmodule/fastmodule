using FastModule.Core.Domain.Constants;
using FastModule.Core.Infrastructure.Persistence;
using FastModule.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace FastModule.EntityFrameworkCore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : FastModuleDbContext<AppDbContext>(options)
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToTable($"{TablePrefix.Prefix}users");

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.SubjectId).IsRequired();
        });
    }
}
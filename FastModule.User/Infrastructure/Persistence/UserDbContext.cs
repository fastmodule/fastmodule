using FastModule.Core.Domain.Constants;
using FastModule.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastModule.User.Infrastructure.Persistence;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
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

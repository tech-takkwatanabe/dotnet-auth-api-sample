using Api.Domain.Entities;
using Api.Domain.VOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
              .HasConversion(
                  id => id.Value,
                  value => new Uuid(value)
              )
              .IsRequired();

        entity.OwnsOne(e => e.Name, name =>
        {
          name.Property(n => n.Value)
              .HasColumnName("Name")
              .HasMaxLength(100)
              .IsRequired();
        });

        entity.OwnsOne(e => e.Email, email =>
        {
          email.Property(e => e.Value)
               .HasColumnName("Email")
               .HasMaxLength(320)
               .IsRequired();
        });

        entity.HasIndex("Email").IsUnique();
      });

      modelBuilder.Entity<RefreshToken>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasConversion(id => id.Value, value => new Uuid(value));
        entity.Property(e => e.UserId).HasConversion(id => id.Value, value => new Uuid(value));
      });
    }
  }
}
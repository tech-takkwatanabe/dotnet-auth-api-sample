using Api.Domain.Entities;
using Api.Domain.VOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<UserEntity>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd(); // long型の主キーはDBで自動生成

        entity.Property(e => e.Uuid)
              .HasConversion(
                  uuid => uuid.Value,
                  value => new Uuid(value)
              )
              .IsRequired();
        entity.HasIndex(e => e.Uuid).IsUnique(); // Uuidはユニークであるべき

        entity.OwnsOne(e => e.Name, name =>
        {
          name.Property(n => n.Value)
              .HasColumnName("Name")
              .HasMaxLength(100)
              .IsRequired();
        });

        entity.OwnsOne(e => e.Email, emailBuilder =>
        {
          emailBuilder.Property(vo => vo.Value)
               .HasColumnName("Email")
               .HasMaxLength(320)
               .IsRequired();
          // Email Value Object の Value プロパティに対してユニーク制約を設定
          emailBuilder.HasIndex(vo => vo.Value).IsUnique().HasDatabaseName("IX_User_Email");
        });

      });

      modelBuilder.Entity<RefreshTokenEntity>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasConversion(id => id.Value, value => new Uuid(value));

        entity.Property(e => e.UserId)
              .HasConversion(
                  uuid => uuid.Value,
                  value => new Uuid(value)
              )
              .IsRequired();

        entity.HasOne<UserEntity>().WithMany().HasForeignKey(rt => rt.UserId).HasPrincipalKey(u => u.Uuid);
      });
    }
  }
}
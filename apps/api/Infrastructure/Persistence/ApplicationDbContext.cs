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
        entity.Property(e => e.Id).ValueGeneratedOnAdd(); // int型の主キーはDBで自動生成

        entity.Property(e => e.Uuid)
              .HasConversion(
                  uuid => uuid.Value, // UuidからGuidへ
                  value => new Uuid(value)  // GuidからUuidへ
              )
              .IsRequired();
        entity.HasIndex(e => e.Uuid).IsUnique(); // Uuidはユニークであるべき

        // Emailプロパティのインデックス名は明示的に指定した方が良い場合がある
        // entity.HasIndex(e => e.Email.Value, "IX_User_Email").IsUnique();
        // ただし、OwnsOne内のプロパティに対するインデックスは以下のように設定

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

        // Email Value Object の Value プロパティに対してユニーク制約を設定
        // EF Core 5.0以降では、Owned Typeのプロパティに対するインデックスはこのように設定
        entity.HasIndex(e => new { e.Email.Value }).IsUnique().HasDatabaseName("IX_User_Email");

      });

      modelBuilder.Entity<RefreshTokenEntity>(entity =>
      {
        entity.HasKey(e => e.Id); // Now correctly refers to RefreshTokenEntity.Id (type Uuid)
        entity.Property(e => e.Id).HasConversion(id => id.Value, value => new Uuid(value)); // Correct for Uuid type PK

        entity.Property(e => e.UserId) // UserId is now Uuid type
              .HasConversion(
                  uuid => uuid.Value, // UuidからGuidへ
                  value => new Uuid(value)  // GuidからUuidへ
              )
              .IsRequired();

        entity.HasOne<UserEntity>().WithMany().HasForeignKey(rt => rt.UserId).HasPrincipalKey(u => u.Uuid); // UserEntity.Uuid を外部キーの参照先とする
      });
    }
  }
}
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Uuidをユニークインデックスとして設定
      modelBuilder.Entity<User>()
          .HasIndex(u => u.Uuid)
          .IsUnique();

      // Emailをユニークインデックスとして設定
      modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
          .IsUnique();

      // DeletedAtに非ユニークインデックスを設定
      modelBuilder.Entity<User>()
          .HasIndex(u => u.DeletedAt);

      // ソフトデリートのためのグローバルクエリフィルター
      // これを設定すると、DbSet<User>をクエリする際、自動的にDeletedAtがNULLのレコードのみが取得されます。
      modelBuilder.Entity<User>()
          .HasQueryFilter(u => u.DeletedAt == null);
    }
  }
}
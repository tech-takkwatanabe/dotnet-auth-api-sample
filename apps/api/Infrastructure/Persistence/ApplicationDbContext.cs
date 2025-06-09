using Api.Domain.Entities;
using Api.Domain.VOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence
{
  public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
  {
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<UserEntity>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedOnAdd();

        entity.Property(e => e.Uuid)
              .HasConversion(
                  uuid => uuid.Value,
                  dbValue => new Uuid(dbValue)
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
          emailBuilder.HasIndex(vo => vo.Value).IsUnique().HasDatabaseName("IX_User_Email");
        });
      });
    }
  }
}
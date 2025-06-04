using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Api.Data
{
  public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
  {
    public ApplicationDbContext CreateDbContext(string[] args)
    {
      // 現在の構成では、docker-compose.yml が apps/api ディレクトリにあるため、
      // プロジェクトのルートディレクトリが apps/api になります。
      // そのため、appsettings.json は自動的に正しく読み込まれます。
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", true)
          .AddEnvironmentVariables()
          .Build();

      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");
      optionsBuilder.UseSqlServer(connectionString);

      return new ApplicationDbContext(optionsBuilder.Options);
    }
  }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL; // SnakeCase burada
using EFCore.NamingConventions;  // <-- BU gerekli

namespace StudentAutomation.Infrastructure.Persistence.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../StudentAutomation.WebAPI");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(cs)
                .UseSnakeCaseNamingConvention() // <-- EFCore.Npgsql 9.x ile gelmeli
                .Options;

            return new DataContext(options);
        }
    }
}
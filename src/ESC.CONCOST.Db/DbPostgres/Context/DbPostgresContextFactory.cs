using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ESC.CONCOST.Db.DbPostgres;

public class DbPostgresContextFactory : IDesignTimeDbContextFactory<DbPostgresContext>
{
    public DbPostgresContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        services.AddDbContext<DbPostgresContext>(options =>
            options
            .UseNpgsql(config.GetConnectionString("DbPostgresConnection"))
            .EnableSensitiveDataLogging()
        );

        var context = services.BuildServiceProvider().GetService<DbPostgresContext>();
        return context;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ESC.CONCOST.Db.DbMssql;
public class DbMssqlContextFactory : IDesignTimeDbContextFactory<DbMssqlContext>
{
    public DbMssqlContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<DbMssqlContext>(options =>
            options
            .UseSqlServer(config.GetConnectionString("DbMssqlConnection"))
            .EnableSensitiveDataLogging()
        );

        var context = services.BuildServiceProvider().GetService<DbMssqlContext>();
        return context;
    }
}
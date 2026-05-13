using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ESC.CONCOST.Db.DbMemory;
public class DbMemoryContextFactory : IDesignTimeDbContextFactory<DbMemoryContext>
{
    public DbMemoryContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<DbMemoryContext>(options =>
            options
            .UseSqlServer(config.GetConnectionString("DbMssqlConnection"))
            .EnableSensitiveDataLogging()
        );

        var context = services.BuildServiceProvider().GetService<DbMemoryContext>();
        return context;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ESC.CONCOST.Db.DbMysql;

public class DbMysqlContextFactory : IDesignTimeDbContextFactory<DbMysqlContext>
{
    public DbMysqlContext CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();

        var connStr = config.GetConnectionString("DbMysqlConnection");
        //var serverVersion = ServerVersion.AutoDetect(connStr);

        services.AddDbContext<DbMysqlContext>(options => options.UseMySQL(connStr)
            .EnableSensitiveDataLogging()
        );

        var context = services.BuildServiceProvider().GetService<DbMysqlContext>();
        return context;
    }
}
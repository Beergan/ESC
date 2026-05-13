using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db.DbMysql;
using System;

namespace ESC.CONCOST.Db;

public class DbMysqlRegister
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config, Action<ModelBuilder> setup)
    {
        DbMysqlContext.SetupAction = setup;

        services.AddDbContext<DbMysqlContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            options.UseMySQL(config.GetConnectionString("DbMysqlConnection"));
        });
        
        //services.AddIdentity<SA_USER, IdentityRole>(options => {
        //    options.SignIn.RequireConfirmedAccount = false;
        //    options.Lockout.AllowedForNewUsers = false;
        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
        //    options.Lockout.MaxFailedAccessAttempts = 3;
        //})
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<DbMysqlContext>()
        //    .AddDefaultTokenProviders();

        services.AddTransient<IDbContext>(provider => provider.GetService(typeof(DbMysqlContext)) as IDbContext);

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DbMysqlContext>();
            //db.Database.EnsureDeleted();
            /* if (db.Database.EnsureCreated())
            {
            } */
        }
    }
}
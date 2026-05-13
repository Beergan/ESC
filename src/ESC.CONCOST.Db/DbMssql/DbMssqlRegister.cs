using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db.DbMssql;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Db;

public class DbMssqlRegister
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config, Action<ModelBuilder> setup)
    {
        DbMssqlContext.SetupAction = setup;

        services.AddDbContext<DbMssqlContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            options.UseSqlServer(config.GetConnectionString("DbMssqlConnection"));
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        services.AddIdentity<SA_USER, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Lockout.AllowedForNewUsers = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            options.Lockout.MaxFailedAccessAttempts = 3;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DbMssqlContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IDbContext>(provider => provider.GetService(typeof(DbMssqlContext)) as IDbContext);

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            //var db = scope.ServiceProvider.GetRequiredService<DbMssqlContext>();
            ///* if (db.Database.EnsureCreated())
            //{
            //} */
            var db = scope.ServiceProvider.GetRequiredService<DbMssqlContext>();
            var seeders = scope.ServiceProvider.GetServices<IEntityRegister>();

            foreach (var seeder in seeders)
            {
                seeder.Seed(db);
            }
            db.SaveChanges();

        }
    }   
}
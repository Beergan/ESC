using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db.DbPostgres;
using System;

namespace ESC.CONCOST.Db;

public class DbPostgresRegister
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config, Action<ModelBuilder> setup)
    {
        DbPostgresContext.SetupAction = setup;

        services.AddDbContext<DbPostgresContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            options.UseNpgsql(config.GetConnectionString("DbPostgresConnection"));
        });

        //services.AddIdentity<SA_USER, IdentityRole>(options => {
        //    options.SignIn.RequireConfirmedAccount = false;
        //    options.Lockout.AllowedForNewUsers = false;
        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
        //    options.Lockout.MaxFailedAccessAttempts = 3;
        //})
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<DbPostgresContext>()
        //    .AddDefaultTokenProviders();

        services.AddTransient<IDbContext>(provider => provider.GetService(typeof(DbPostgresContext)) as IDbContext);

        //using (var scope = services.BuildServiceProvider().CreateScope())
        //{
        //    var db = scope.ServiceProvider.GetRequiredService<DbPostgresContext>();
        //    db.Database.EnsureDeleted();
        //    if (db.Database.EnsureCreated())
        //    {
        //    }
        //}
    }
}
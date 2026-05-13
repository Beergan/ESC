using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db.DbMemory;
using System;

namespace ESC.CONCOST.Db;

public class DbMemoryRegister
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration config, Action<ModelBuilder> setup)
    {
        DbMemoryContext.SetupAction = setup;

        services.AddDbContext<DbMemoryContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            options.UseInMemoryDatabase("MyDb");
        });

        //services.AddIdentity<SA_USER, IdentityRole>(options => {
        //    options.SignIn.RequireConfirmedAccount = false;
        //    options.Lockout.AllowedForNewUsers = false;
        //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
        //    options.Lockout.MaxFailedAccessAttempts = 3;
        //})
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<DbMemoryContext>()
        //    .AddDefaultTokenProviders();

        services.AddTransient<IDbContext>(provider => provider.GetService(typeof(DbMemoryContext)) as IDbContext);

        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DbMemoryContext>();
            /* if (db.Database.EnsureCreated())
            {
            } */
        }
    }
}
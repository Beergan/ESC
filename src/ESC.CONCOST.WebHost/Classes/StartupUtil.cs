using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;
using ESC.CONCOST.ModuleSettingCore;

namespace ESC.CONCOST.WebHost;

public static class StartupUtil
{
    public static async Task SetupDefaultUsers(string ternantId, IDbContext dbContext, IServiceProvider serviceProvider)
    {
        if(dbContext.Set<SA_USER>().Any())
        {
            return;
        }

        var adminRole = new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "ADMIN", ConcurrencyStamp = "fab4fac1-c546-41de-aebc-a14da6895711", NormalizedName = "ADMIN" };
        var CustomerRole = new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895712", Name = "CUSTOMER", ConcurrencyStamp = "fab4fac1-c546-41de-aebc-a14da6895712", NormalizedName = "CUSTOMER" }; 
        dbContext.Set<IdentityRole>().AddRange(adminRole);

        var user1 = new SA_USER
        {
            Id = "6f449fa3-3964-474b-b94b-efff192ef2ca",
            Email = "admin@gmail.com",
            UserName = "admin@gmail.com",
            FirstName = "Admin",
            LastName = "User",
            Avatar = "/upload/avatar/male-01.jpeg",
            NormalizedUserName = "ADMIN",
            NormalizedEmail = "ADMIN",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            Active = true,
            Logo = "/upload/logo/logo-ccs-orange.png",
            CompanyGuid = new Guid("88888888-8888-4888-a888-000000000001"),
            Note = "System Administrator",
            ApprovedBy   = "system",            // Required — NOT NULL
            ApprovedDate = DateTime.UtcNow,     // Required — NOT NULL nullable
        };
        user1.PasswordHash = new PasswordHasher<SA_USER>().HashPassword(user1, "UserAdmin@789");

        dbContext.Set<SA_USER>().AddRange(user1);

        dbContext.Set<IdentityUserRole<string>>().AddRange
        (
            new IdentityUserRole<string> { RoleId = adminRole.Id, UserId = user1.Id.ToString() }
        );

        using (var scope = serviceProvider.CreateScope())
        {
            var sp = scope.ServiceProvider;
            var ctx = sp.GetRequiredService<IMyContext>();

            var listPermissions = GlobalPermissions.Dictionary.ToList();

            foreach (var item in listPermissions.EmptyIfNull())
            {
                var claim = new IdentityRoleClaim<string>()
                {
                    RoleId = adminRole.Id,
                    ClaimType = item.Key.Name,
                    ClaimValue = Convert.ToString(item.Value.Sum(x => x.Item1))
                };
                
                await dbContext.Set<IdentityRoleClaim<string>>().AddAsync(claim);
            }

            await dbContext.SaveChangesAsync();
        }
    }


    public static async Task CreateOrUpdateTernant(this IServiceProvider serviceProvider, string ternantId) 
    {
        Console.WriteLine("========================================================================================================================");
        Console.WriteLine($"SETUP DATABASE FOR TENANT {ternantId}");

        using (var serviceScope = serviceProvider.CreateScope())
        {
            var sp = serviceScope.ServiceProvider;

            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = string.Format(config.GetConnectionString("DbMssqlConnection"), ternantId);

            var dbContext = sp.GetRequiredService<IDbContext>();
            dbContext.Database.SetConnectionString(connectionString);

            if (dbContext.Database.GetMigrations().Count() > 0)
            {
                dbContext.Database.Migrate();
                await StartupUtil.SetupDefaultUsers(ternantId, dbContext, serviceProvider);
            }

            dbContext.SaveChanges();
        }
    }
}

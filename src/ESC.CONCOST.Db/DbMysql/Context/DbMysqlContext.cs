using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Base;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Db.DbMysql;

public partial class DbMysqlContext : /*IdentityDbContext<SA_USER>,*/ DbContext, IDbContext
{
    public static Action<ModelBuilder> SetupAction { get; set; }

    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public Guid EmployeeGuid { get; set; }

    public DbMysqlContext(DbContextOptions<DbMysqlContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.Entity<IdentityUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        //builder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
        //builder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

        //builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        //builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

        //builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
        //builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
        //builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
        //builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

        //builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

        //builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
        //builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
        //builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

        //builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        //builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
        //builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
        //builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
        
        SetupAction?.Invoke(builder);
        //builder.SeedData();
    }

    public IRepository<T> Repo<T>() where T : class
    {
        return new BaseRepository<T>(this);
    }

    public Task<int> SaveChangesAsync(Guid guidCntr, string method)
    {
        return this.SaveChangesAsync();
    }

    public Task<int> SaveChangesAsync(string method, Guid guid, string cntrNo = "-")
    {
        return this.SaveChangesAsync();
    }
}
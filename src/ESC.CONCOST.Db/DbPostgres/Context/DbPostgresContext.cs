using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Base;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Db.DbPostgres;

public partial class DbPostgresContext : /*IdentityDbContext<SA_USER>,*/ DbContext, IDbContext
{
    public static Action<ModelBuilder> SetupAction { get; set; }

    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public Guid EmployeeGuid { get; set; }

    public DbPostgresContext(DbContextOptions<DbPostgresContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
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
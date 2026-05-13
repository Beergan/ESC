using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Base;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Db.DbMemory;

public class DbMemoryContext : /*IdentityDbContext<SA_USER>,*/ DbContext, IDbContext
{
    public static Action<ModelBuilder> SetupAction { get; set; }

    public string UserId { get; set; }
    public string IpAddress { get; set; }
    public Guid EmployeeGuid { get; set; }

    public DbMemoryContext(DbContextOptions<DbMemoryContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        SetupAction?.Invoke(builder);
        //builder.SeedData();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(string.Format("Server=(localdb)\\mssqllocaldb;Database=Tenant_{0};Trusted_Connection=True;MultipleActiveResultSets=true", _tenant.Host));
    //}

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
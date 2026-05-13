using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base;

public interface IDatabaseTransaction : IDisposable
{
    IDbContext Db { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    IRepository<T> Repo<T>() where T : class;

    Task SaveChangesAsync();

    Task SaveChangesAsync(string eventName);

    //Task SaveChangesAsync(EntityBizCntrStock cntr, string eventName);

    void Commit();

    void Rollback();

    EntityEntry Attach(object entity);
}
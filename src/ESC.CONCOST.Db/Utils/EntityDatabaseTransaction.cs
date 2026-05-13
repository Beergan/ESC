using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Db;

public class EntityDatabaseTransaction : IDatabaseTransaction
{
    private bool _disposed;
    private IDbContext _dbContext;
    private IDbContextTransaction _transaction;

    public EntityDatabaseTransaction(IDbContext ctx)
    {
        _dbContext = ctx;
        _transaction = ctx.Database.BeginTransaction();
    }

    public IDbContext Db => _dbContext;

    public DbSet<T> Set<T>() where T : class
    {
        return _dbContext.Set<T>();
    }

    public IRepository<T> Repo<T>() where T : class
    {
        return new BaseRepository<T>(_dbContext);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();

        //foreach (var entry in _dbContext.ChangeTracker.Entries().ToArray())
        //{
        //    entry.State = EntityState.Detached;
        //}
    }

    public async Task SaveChangesAsync(string eventName)
    {
        //await _dbContext.SaveChangesAsync(Guid.Empty, eventName);

        //foreach (var entry in _dbContext.ChangeTracker.Entries().ToArray())
        //{
        //    entry.State = EntityState.Detached;
        //}
        await Task.CompletedTask;
    }

    public void Commit()
    {
        _transaction.Commit();
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public EntityEntry Attach(object entity)
    {
        return _dbContext.Attach(entity);
    }
}
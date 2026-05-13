using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ESC.CONCOST.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ESC.CONCOST.Db;

public class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly Func<IDbContext> _dbFactory;

    private IDbContext _dbContext;
    protected DbSet<T> DbSet => _dbContext.Set<T>();

    public BaseRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual IQueryable<T> Query()
    {
        return DbSet.AsNoTracking();
    }

    public virtual IQueryable<T> Query(Expression<Func<T, bool>> filter)
    {
        return DbSet.AsNoTracking().Where(filter);
    }

    public virtual async Task<bool> Exists(Expression<Func<T, bool>> filter)
    {
        return await this.Query().AnyAsync(filter);
    }

    public virtual async Task<bool> NotExists(Expression<Func<T, bool>> filter)
    {
        return !await this.Query().AnyAsync(filter);
    }

    public virtual async Task<T> GetOne()
    {
        return await this.Query().FirstOrDefaultAsync();
    }

    public async Task<T> GetOne(Expression<Func<T, bool>> filter)
    {
        return await this.Query().Where(filter).FirstOrDefaultAsync();
    }

    public virtual IQueryable<T> QueryEdit(Expression<Func<T, bool>> filter)
    {
        return DbSet.Where(filter);
    }

    public async Task<T> GetOneEdit(Expression<Func<T, bool>> filter)
    {
        return await this.DbSet.Where(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<T> GetOne(object id)
    {
        var entity = await DbSet.FindAsync(id);
        //_dbContext.Entry(entity).State = EntityState.Detached;

        return await Task.FromResult<T>(entity);
    }

    public async Task<IList<T>> GetList()
    {
        return await this.Query().ToListAsync();
    }

    public async Task<IList<T>> GetList(Expression<Func<T, bool>> filter)
    {
        return await this.Query(filter).ToListAsync();
    }

    public virtual async Task Insert(T entity, bool commit = true)
    {
        // Ngắt navigation để tránh bị insert trùng
        foreach (var entry in _dbContext.Entry(entity).Navigations)
        {
            if (entry.Metadata.IsCollection)
            {
                // Với collection navigation (vd: BookingServices) thì giữ nguyên
                continue;
            }

            // Nếu là reference navigation (vd: Customer, Pet...)
            if (entry.CurrentValue != null)
            {
                // Đảm bảo EF không cố insert entity con
                _dbContext.Entry(entry.CurrentValue).State = EntityState.Unchanged;
            }
        }

        await DbSet.AddAsync(entity);

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
        }
    }


    public virtual async Task InsertRange(params T[] entities)
    {
        DbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync();

        //foreach (var entity in entities)
        //    _dbContext.Entry(entity).State = EntityState.Detached;
    }

    public virtual async Task Update(T entity, bool commit = true)
    {
        _dbContext.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
    }


    public virtual async Task UpdateICollection(T entity, bool commit = true)
    {
        _dbContext.ChangeTracker.Clear();

        DbSet.Update(entity);

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
    public virtual async Task UpdateRange(IEnumerable<T> entities, bool commit = true)
    {
        foreach (var entity in entities)
        {
            var idProperty = typeof(T).GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);

            var existingEntity = await DbSet.FindAsync(entityId);

            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

                foreach (var navigation in _dbContext.Entry(existingEntity).Navigations)
                {
                    if (!navigation.IsLoaded) continue;

                    var newValue = _dbContext.Entry(entity).Member(navigation.Metadata.Name).CurrentValue;
                    navigation.CurrentValue = newValue;
                }
            }
            else
            {
                DbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
        }
    }


    //public virtual async Task UpdateList(List<T> listEntity, bool commit = true)
    //{
    //    foreach (var entity in listEntity.DefaultIfEmpty())
    //    {
    //        var idProperty = typeof(T).GetProperty("Id");
    //        var entityId = idProperty?.GetValue(entity);

    //        var existingEntity = await DbSet.FindAsync(entityId);

    //        if (existingEntity != null)
    //        {
    //            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
    //        }
    //        else
    //        {
    //            DbSet.Attach(entity);
    //            _dbContext.Entry(entity).State = EntityState.Modified;
    //        }

    //        if (commit)
    //        {
    //            await _dbContext.SaveChangesAsync();
    //            _dbContext.Entry(entity).State = EntityState.Detached;
    //        }
    //    }
    //}

    public virtual async Task Remove(T entity, bool commit = true)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
            //_dbContext.Entry(entity).State = EntityState.Detached;
        }
    }
    public virtual async Task RemoveRange(IEnumerable<T> entities, bool commit = true)
    {
        DbSet.RemoveRange(entities);

        if (commit)
        {
            await _dbContext.SaveChangesAsync();
        }
    }


    public EntityEntry Attach(T entity)
    {
        return DbSet.Attach(entity);
    }
}
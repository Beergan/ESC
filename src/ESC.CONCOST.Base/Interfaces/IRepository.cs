using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base;

public interface IRepository<T> where T : class
{
    Task<bool> Exists(Expression<Func<T, bool>> filter);

    Task<bool> NotExists(Expression<Func<T, bool>> filter);

    IQueryable<T> Query();

    IQueryable<T> Query(Expression<Func<T, bool>> filter = null);

    IQueryable<T> QueryEdit(Expression<Func<T, bool>> filter = null);

    Task<T> GetOne();

    Task<T> GetOne(object id);

    Task<T> GetOne(Expression<Func<T, bool>> filter);

    Task<T> GetOneEdit(Expression<Func<T, bool>> filter);

    Task<IList<T>> GetList();

    Task<IList<T>> GetList(Expression<Func<T, bool>> filter);

    Task Insert(T entity, bool commit = true);
    Task RemoveRange(IEnumerable<T> entities, bool commit = true);
    Task InsertRange(params T[] entity);
    Task UpdateRange(IEnumerable<T> entities, bool commit = true);
    Task Update(T entity, bool commit = true);

    Task Remove(T entity, bool commit = true);


    Task UpdateICollection(T entity, bool commit = true);

    EntityEntry Attach(T estimate);

}
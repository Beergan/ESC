using Hangfire;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Base;

namespace ESC.CONCOST.Db;

public class CacheRepository<T> : BaseRepository<T>, ICacheRepository<T> where T : class
{
    private readonly static CacheMode cacheMode = CacheMode.Memory;
    private readonly string cacheKey = $"{typeof(T)}";
    private readonly Func<CacheMode, ICacheService> _cacheService;

    public CacheRepository(IDbContext dbContext, Func<CacheMode, ICacheService> cacheService) : base(dbContext)
    {
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<T>> GetListWithCache()
    {
        if (!_cacheService(cacheMode).TryGet(cacheKey, out IReadOnlyList<T> cachedList))
        {
            cachedList = await base.Query().ToListAsync();
            _cacheService(cacheMode).Set(cacheKey, cachedList);
        }

        return cachedList;
    }
    public async Task<T> InsertWithCache(T entity)
    {
        await base.Insert(entity);
        var cachedList = await base.Query().ToListAsync();
        _cacheService(cacheMode).Set(cacheKey, cachedList);
        return entity;
    }

    public async Task UpdateWithCache(T entity)
    {
        await base.Update(entity);
        BackgroundJob.Enqueue(() => RefreshCache());
    }

    public async Task RemoveWithCache(T entity)
    {
        await base.Remove(entity);
        BackgroundJob.Enqueue(() => RefreshCache());
    }

    public async Task RefreshCache()
    {
        _cacheService(cacheMode).Remove(cacheKey);
        var cachedList = await base.Query().ToListAsync();
        _cacheService(cacheMode).Set(cacheKey, cachedList);
    }
}
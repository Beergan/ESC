// -------------------------------------------------------------------------------------------------
// Copyright (c) Johan Boström. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base;

#pragma warning disable EF1001 // Internal EF Core API usage.
public interface IDbContext : IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbContextPoolable
{
    Microsoft.EntityFrameworkCore.DbContextId ContextId { get; }
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    EntityEntry Add(object entity);
    EntityEntry<TEntity> Add<TEntity>([NotNullAttribute] TEntity entity) where TEntity : class;
    ValueTask<EntityEntry> AddAsync([NotNullAttribute] object entity, CancellationToken cancellationToken = default);
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>([NotNullAttribute] TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
    void AddRange(IEnumerable<object> entities);
    void AddRange(params object[] entities);
    Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default(CancellationToken));
    Task AddRangeAsync(params object[] entities);
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry Attach(object entity);
    void AttachRange(params object[] entities);
    void AttachRange(IEnumerable<object> entities);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry Entry(object entity);
    bool Equals(object obj);
    object Find([NotNullAttribute] Type entityType,  params object[] keyValues);
    TEntity Find<TEntity>( params object[] keyValues) where TEntity : class;
    ValueTask<object> FindAsync([NotNullAttribute] Type entityType,  object[] keyValues, CancellationToken cancellationToken);
    ValueTask<object> FindAsync([NotNullAttribute] Type entityType,  params object[] keyValues);
    ValueTask<TEntity> FindAsync<TEntity>( object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
    ValueTask<TEntity> FindAsync<TEntity>( params object[] keyValues) where TEntity : class;
    int GetHashCode();
    EntityEntry Remove(object entity);
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
    void RemoveRange(IEnumerable<object> entities);
    void RemoveRange(params object[] entities);
    int SaveChanges(bool acceptAllChangesOnSuccess);
    int SaveChanges();
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    //Task<int> SaveChangesAsync(Guid guidCntr, string method);
    //Task<int> SaveChangesAsync(string method, Guid guid, string cntrNo = "-");
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    string ToString();
    EntityEntry Update(object entity);
    EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;
    void UpdateRange(params object[] entities);
    void UpdateRange(IEnumerable<object> entities);
    IRepository<T> Repo<T>() where T : class; 
    string UserId { get; set; }
    string IpAddress { get; set; }
    Guid EmployeeGuid { get; set; }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
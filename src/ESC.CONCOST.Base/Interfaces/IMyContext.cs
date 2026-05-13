using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base;

public interface IMyContext
{
    HttpContext HttpContext { get; }

    T GetService<T>();

    IDatabaseTransaction BeginTransaction();

    IDatabaseTransaction BeginTransaction(string userId);

    IDbContext ConnectDb();

    ICacheRepository<T> Cache<T>() where T : class;

    //AppManifest AppManifest { get; }

    ITextTranslator Text { get; }

    IMediator Mediator { get; }

    bool CheckPermission<T>(params T[] claims) where T : Enum;

    string UserId { get; }

    string EnterpriseCode { get; }

    SA_USER GetCurrentUser();

    string IpAddress { get; }

    string UserAgent { get; }

    string Summary { get; }

    Guid GuidEmployee { get; }
    Guid GuidCompany { get; }

    Task PublishEvent(string evt, params string[] data);
    void PublishEventStatic(string evt, params string[] data);

    INotifyService Notifier { get; }

    //ICollection<BlazorSession> BlazorSessions { get; }

    //IWorkFlowService Workflow { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    IRepository<T> Repo<T>() where T : class;
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base;

public class MyServiceBase : IServiceBase
{
    protected readonly IMyContext _ctx;

    public MyServiceBase(IMyContext ctx)
    {
        _ctx = ctx;
    }

    [HttpGet]
    public Task<List<KeyValuePair<FeatureModel, Tuple<long, string, string>[]>>> GetListPermissions()
    {
        var list = GlobalPermissions.Dictionary.ToList();
        return Task.FromResult(list);
    }

    [HttpGet]
    public Task<List<OptionItem<Guid>>> GetOptionOffices()
    {
        return _ctx.Mediator.Send(new QueryOptionOffices());
    }

    [HttpGet]
    public Task<List<OptionItem<Guid>>> GetOptionJob()
    {
        return _ctx.Mediator.Send(new QueryOptionJob());
    }

    [HttpGet]
    public Task<OptionItem<Guid>> GetOptionCompany()
    {
        return _ctx.Mediator.Send(new QueryOptionCompany());
    }

    [HttpGet]
    public Task<ModelInfoEmployee> GetInfoEmployee(Guid guid)
    {
        return _ctx.Mediator.Send(new QueryInfoEmployee { Guid = guid });
    }

    [HttpGet]
    public Task<List<ModelService>> GetListService()
    {
        return _ctx.Mediator.Send(new QueryListService());
    }

    protected IQueryable<T> ApplyBranchFilter<T>(IQueryable<T> query, Enum permission, Expression<Func<T, Guid>> companyGuidSelector)
    {
        if (_ctx.CheckPermission(permission))
            return query;

        var userCompanyGuid = _ctx.GuidCompany;

        var parameter = companyGuidSelector.Parameters[0];
        var body = Expression.Equal(companyGuidSelector.Body, Expression.Constant(userCompanyGuid));
        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        return query.Where(lambda);
    }

    protected Result Ok(string message = "")
    {
        return new Result { Success = true, Message = message };
    }

    protected Result Fail(string message = "")
    {
        return new Result { Success = false, Message = message };
    }
}
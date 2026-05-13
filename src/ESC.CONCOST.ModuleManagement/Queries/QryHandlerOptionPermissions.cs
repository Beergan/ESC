using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;

namespace ESC.CONCOST.ModuleSetting;

public class QryHandlerOptionPermissions : IRequestHandler<QueryOptionPermissions, List<OptionItem<Guid>>>
{
    private readonly IMyContext _ctx;

    public QryHandlerOptionPermissions(IMyContext ctx)
    {
        _ctx = ctx;
    }

    public Task<List<OptionItem<Guid>>> Handle(QueryOptionPermissions request, CancellationToken cancellationToken)
    {
        return _ctx.Repo<IdentityRole>()
            .Query(x => x.Id != "fab4fac1-c546-41de-aebc-a14da6895711")
            .Select(x => new OptionItem<Guid> { Value = Guid.Parse(x.Id), Text = x.Name })
            .AsNoTracking()
            .ToListAsync();
    }
}
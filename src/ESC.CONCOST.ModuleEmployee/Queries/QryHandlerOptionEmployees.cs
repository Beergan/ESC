using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleEmployeeCore;

namespace ESC.CONCOST.ModuleEmployee;

public class QryHandlerOptionEmployees : MyServiceBase, IRequestHandler<QueryOptionEmployees, List<OptionItem<Guid>>>
{
    public QryHandlerOptionEmployees(IMyContext ctx) : base(ctx)
    {

    }

    public async Task<List<OptionItem<Guid>>> Handle(QueryOptionEmployees request, CancellationToken cancellationToken)
    {
        return await _ctx.Repo<EntityEmployee>()
            .Query()
            .Select(x => new OptionItem<Guid> { Value = x.Guid, Text = x.FullName})
            .AsNoTracking()
            .ToListAsync();
    }
}
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

public class QryHandlerCheckJobExists : MyServiceBase, IRequestHandler<CheckJobExists, bool>
{
    public QryHandlerCheckJobExists(IMyContext ctx) : base(ctx)
    {

    }

    public Task<bool> Handle(CheckJobExists request, CancellationToken cancellationToken)
    {
        bool result = _ctx.Repo<EntityEmployee>().Query().Any();

        return Task.FromResult(result);
    }
}
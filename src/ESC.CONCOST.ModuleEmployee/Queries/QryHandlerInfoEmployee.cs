using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleEmployeeCore;


namespace ESC.CONCOST.ModuleEmployee;

public class QryHandlerInfoEmployee : IRequestHandler<QueryInfoEmployee, ModelInfoEmployee>
{
    private readonly IMyContext _ctx;
    public QryHandlerInfoEmployee(IMyContext ctx)
    {
        _ctx = ctx;
    }
    public Task<ModelInfoEmployee> Handle(QueryInfoEmployee request, CancellationToken cancellationToken)
    {
        return _ctx.Repo<EntityEmployee>()
            .Query(x => x.Guid == request.Guid)
            .Select(x => new ModelInfoEmployee
            {
                Guid = x.Guid,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.FullName,
                Gender = x.Gender,
                Phone = x.Phone,
                Email = x.Email,
            })
            .FirstOrDefaultAsync();
    }
}
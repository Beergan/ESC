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

public class QryHandlerListEmployeeAccount : IRequestHandler<QueryListEmployeeAccount, List<ModelEmployeeAccount>>
{
    private readonly IMyContext _ctx;
    public QryHandlerListEmployeeAccount(IMyContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<List<ModelEmployeeAccount>> Handle(QueryListEmployeeAccount request, CancellationToken cancellationToken)
    {
        var employees = await _ctx.Set<EntityEmployee>()
        .GroupJoin(
            _ctx.Set<SA_USER>(),
            emp => emp.Guid,
            acc => acc.GuidEmployee,
            (Employee, AccountGroup) => new { Employee, AccountGroup })
        .AsNoTracking()
        .SelectMany(
            x => x.AccountGroup.DefaultIfEmpty(),
            (row, acc) => new ModelEmployeeAccount
            {
                DateCreated = row.Employee.DateCreated,
                GuidEmployee = row.Employee.Guid,
                DateOfBirth = row.Employee.DateOfBirth,
                FirstName = row.Employee.FirstName,
                LastName = row.Employee.LastName,
                Email = row.Employee.Email,
                Phone = row.Employee.Phone,
                UserName = acc.UserName ?? "-",
                Locked = string.IsNullOrWhiteSpace(acc.UserName) ? null : row.Employee.Active,
            }
        ).OrderByDescending(x => x.DateCreated).ToListAsync();

        return employees;
    }
}
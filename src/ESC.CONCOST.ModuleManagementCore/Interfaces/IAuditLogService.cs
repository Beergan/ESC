using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleManagementCore;

[BasePath("api/AuditLog")]
public interface IAuditLogService : IServiceBase
{
    [Post(nameof(GetList))]
    Task<ResultsOf<AuditLog>> GetList();

    
    [Post(nameof(CheckAuditLog))]
    Task<bool> CheckAuditLog();
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;

namespace ESC.CONCOST.ModuleManagement;

public class AuditLogService : MyServiceBase, IAuditLogService
{
    private readonly ILogger<AuditLogService> _log;

    //-Dev-Bee-CN: Khởi tạo service nhật ký hệ thống (đã xong)
    public AuditLogService(IMyContext ctx, ILogger<AuditLogService> logger) : base(ctx)
    {
        _log = logger;
    }

    //-Dev-Bee-CN: Lấy danh sách toàn bộ nhật ký hệ thống (đã xong)
    public async Task<ResultsOf<AuditLog>> GetList()
    {
        var list = await _ctx.Repo<AuditLog>().Query().AsNoTracking().ToListAsync();

        return ResultsOf<AuditLog>.Ok(list);
    }

    
    //-Dev-Bee-CN: Kiểm tra quyền xem nhật ký (đã xong)
    public Task<bool> CheckAuditLog()
    {
        return Task.FromResult(_ctx.CheckPermission(PERMISSION.AUDIT_LOG));
    }
}
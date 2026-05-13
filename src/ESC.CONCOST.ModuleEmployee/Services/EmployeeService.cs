using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.ModuleEmployeeCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ESC.CONCOST.Base;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ESC.CONCOST.ModuleEmployee;

public class EmployeeService : MyServiceBase, IEmployeeService
{
    private IWebHostEnvironment hostingEnv;
    private readonly ILogger<EmployeeService> _log;
    private readonly string _enterpriseCode;

    public EmployeeService(IMyContext ctx, ILogger<EmployeeService> logger, IWebHostEnvironment env) : base(ctx)
    {
        hostingEnv = env;
        _log = logger;
        _enterpriseCode = _ctx.EnterpriseCode;
    }

    public async Task<ResultOf<EntityEmployee>> Get(Guid guid)
    {
        if (!_ctx.CheckPermission(PERMISSION.FILE_EMPLOYEE_VIEW))
            return ResultOf<EntityEmployee>.Error(_ctx.Text["Bạn không có quyền!", "권한이 없습니다!"]);
       using (var db = _ctx.ConnectDb()){
            try
            {
                var data = await db.Repo<EntityEmployee>().Query(t => t.Guid == guid)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();

                return ResultOf<EntityEmployee>.Ok(data);
            }
            catch (Exception ex)
            {
                _log.LogError($"{_ctx.Summary} - {ex.Message}");
                return ResultOf<EntityEmployee>.Error(_ctx.Text["Đã có lỗi xảy ra!", "오류가 발생했습니다!"]);
            }
        }
    }

    /// <summary>
    /// Lấy toàn bộ thông tin hồ sơ và danh mục lựa chọn (Tối ưu kết hợp: JOIN + Sequential Await).
        public async Task<ResultOf<ModelEmployeeProfile>> GetProfile(Guid guid)
    {
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var result = new ModelEmployeeProfile();

                // 1. Lấy Employee đơn giản
                result.Employee = await db.Repo<EntityEmployee>().Query().AsNoTracking()
                    .Where(e => e.Guid == guid)
                    .FirstOrDefaultAsync() ?? new();

                return ResultOf<ModelEmployeeProfile>.Ok(result);
            }
            catch (Exception ex)
            {
                _log.LogError($"{_ctx.Summary} - {ex.Message}");
                return ResultOf<ModelEmployeeProfile>.Error(
                    _ctx.Text["Đã có lỗi xảy ra khi nạp hồ sơ!", "프로필 로딩 중 오류가 발생했습니다!"]);
            }
        }
    }

    public async Task<ResultsOf<EntityEmployee>> GetList()
    {
        if (!_ctx.CheckPermission(PERMISSION.EMPLOYEE_VIEW))
            return ResultsOf<EntityEmployee>.Error(_ctx.Text["Bạn không có quyền!", "권한이 없습니다!"]);

        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var data = await db.Repo<EntityEmployee>().Query().AsNoTracking().ToListAsync();
                return ResultsOf<EntityEmployee>.Ok(data);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return ResultsOf<EntityEmployee>.Error(_ctx.Text["Đã có lỗi xảy ra!", "오류가 발생했습니다!"]);
            }
        }
    }

    public async Task<Result> Save([Body] EntityEmployee info)
    {
        if (!_ctx.CheckPermission(PERMISSION.EMPLOYEE_CREATE_UPDATE))
            return Result.Error(_ctx.Text["Bạn không có quyền!", "권한이 없습니다!"]);

        using (var db = _ctx.ConnectDb())
        {
            try
            {
                if (info.Id > 0)
                {
                    info.DateModified = DateTime.UtcNow;
                    await db.Repo<EntityEmployee>().Update(info);

                    var user = await db.Repo<SA_USER>().Query().FirstOrDefaultAsync(x => x.GuidEmployee == info.Guid);
                    if (user != null)
                    {
                        user.LastName = info.LastName;
                        user.FirstName = info.FirstName;
                        user.Email = info.Email;
                        user.PhoneNumber = info.Phone;

                        await db.Repo<SA_USER>().Update(user);
                    }
                }
                else
                {
                    if (info.Guid == Guid.Empty)
                    {
                        await db.Repo<EntityEmployee>().Insert(info);
                    }
                }

                await db.SaveChangesAsync();

                return Result.Ok(_ctx.Text["Lưu nhân viên thành công!", "직원 정보가 성공적으로 저장되었습니다!"]);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, _ctx.Summary);
                return Result.Error(_ctx.Text["Đã có lỗi xảy ra trong quá trình lưu!", "저장 중 lỗi가 발생했습니다!"]);
            }
        }
    }


    public async Task<Result> SetToActiveEmployee(int id, Guid guidEmployee)
    {
        if (!_ctx.CheckPermission(PERMISSION.EMPLOYEE_ACTIVE_ACCOUNT))
            return Result.Error(_ctx.Text["Bạn không có quyền!", "권한이 없습니다!"]);

        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var item = await _ctx.Repo<EntityEmployee>().GetOne(id);
                item.Active = !item.Active;
                item.DateModified = DateTime.UtcNow;
                await db.Repo<EntityEmployee>().Update(item);

                var user = await db.Repo<SA_USER>().Query().FirstOrDefaultAsync(x => x.GuidEmployee == guidEmployee);
                if (user != null)
                {
                    user.Active = item.Active;
                    await db.Repo<SA_USER>().Update(user);
                }
                return Result.Ok(_ctx.Text["Cập nhật trạng thái thành công!", "상태가 성공적으로 업데이트되었습니다!"]);
            }
            catch (Exception ex)
            {
                _log.LogError($"{_ctx.Summary} - {ex.Message}");
                return Result.Error(_ctx.Text["Đã có lỗi xảy ra!", "오류가 발생했습니다!"]);
            }
        }
    }




    

    public async Task<Result> Delete(int id)
    {
        if (!_ctx.CheckPermission(PERMISSION.EMPLOYEE_CREATE_UPDATE))
            return Result.Error(_ctx.Text["Bạn không có quyền!", "권한이 없습니다!"]);
            
        using (var db = _ctx.ConnectDb())
        {
            try
            {
                var item = await db.Repo<EntityEmployee>().GetOne(id);
                if (item == null) return Result.Ok();

                // 2. Kiểm tra tài khoản đăng nhập còn active
                var activeUser = await db.Repo<SA_USER>().Query().AsNoTracking().FirstOrDefaultAsync(x => x.GuidEmployee == item.Guid);
                if (activeUser != null && activeUser.Active)
                    return Result.Error(_ctx.Text["Không thể xóa nhân viên có tài khoản đang hoạt động!", "활성 계정이 있는 직원은 삭제할 수 없습니다!"]);


                // 4. Xóa lịch sử chức danh/vị trí
                //var history = await db.Set<EntityEmployeePositionHistory>().Where(x => x.EmployeeGuid == item.Guid).ToListAsync();
                //db.Set<EntityEmployeePositionHistory>().RemoveRange(history);


                // 6. Gỡ liên kết tài khoản (nếu có)
                if (activeUser != null)
                {
                    activeUser.GuidEmployee = null;
                    activeUser.EmployeeConnected = false;
                    await db.Repo<SA_USER>().Update(activeUser);
                }

                // 7. Xóa nhân viên chính thức
                await db.Repo<EntityEmployee>().Remove(item);
                
                await db.SaveChangesAsync();

                return Result.Ok(_ctx.Text["Đã xoá nhân sự", "직원이 삭제되었습니다"]);
            }
            catch (Exception ex)
            {
                _log.LogError($"{_ctx.Summary} - {ex.Message}");
                return Result.Error(_ctx.Text["Đã có lỗi xảy ra!", "오류가 발생했습니다!"]);
            }
        }
    }
}
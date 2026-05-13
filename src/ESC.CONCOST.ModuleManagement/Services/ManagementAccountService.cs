using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestEase;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleManagement;

public class ManagementAccountService : MyServiceBase, IManagementAccountService
{
    private readonly ILogger<ManagementAccountService> _log;
    private readonly IMailSettingService _svcMailSetting;

    public ManagementAccountService(IMyContext ctx, ILogger<ManagementAccountService> logger, IMailSettingService mailSettingService) : base(ctx)   
    {
        _log = logger;
        _svcMailSetting = mailSettingService;
    }

    public async Task<ResultsOf<OptionItem<Guid>>> GetListRoles()
    {
        var list = await _ctx.Repo<IdentityRole>()
            .Query(x => x.Id != "fab4fac1-c546-41de-aebc-a14da6895711")
            .Select(x => new OptionItem<Guid>
            {
                Value = Guid.Parse(x.Id),
                Text = x.Name,
            })
        .AsNoTracking()
        .ToListAsync();

        return ResultsOf<OptionItem<Guid>>.Ok(list);
    }

    public async Task<ResultsOf<ModelUserAccount>> GetList()
    {
        try
        {
            var users = await _ctx.Set<SA_USER>()
            .Select(x => new ModelUserAccount
            {
                UserName = x.UserName,
                Email = x.Email,
                GuidEmployee = x.GuidEmployee,
                EmployeeConnected = x.EmployeeConnected,
                Avatar = x.Avatar
            }).AsNoTracking().ToListAsync();

            return ResultsOf<ModelUserAccount>.Ok(users);
        }
        catch (Exception ex)
        {
            _log.LogError(ex.Message);
            return ResultsOf<ModelUserAccount>.Error("Đã có lỗi xảy ra!");
        }
    }

    public async Task<ResultsOf<ModelEmployeeAccount>> GetListEmployeeAccount()
    {
        try
        {
            var data = await _ctx.Mediator.Send(new QueryListEmployeeAccount());

            return ResultsOf<ModelEmployeeAccount>.Ok(data);
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return ResultsOf<ModelEmployeeAccount>.Error("Đã có lỗi xảy ra!");
        }
    }

    public async Task<ResultOf<string>> CreateUser([Body] ModelUserAccount model, string BaseUri)
    {
        try
        {
            var check = await _ctx.Set<SA_USER>().AnyAsync(x => x.UserName == model.UserName);

            if (check)
                return ResultOf<string>.Error("Tài khoản đã tồn tại!");

            //Tạo tài khoản
            var user = new SA_USER()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                Avatar = model.Avatar  == null ? "" : model.Avatar,
                PhoneNumber = model.Phone,
                GuidEmployee = model.GuidEmployee,
                EmployeeConnected = true,
                Active = true,
                Note = "",
                Logo = "",
            };
            var userMgr = _ctx.GetService<UserManager<SA_USER>>();

            user.PasswordHash = userMgr.PasswordHasher.HashPassword(user, model.Password);
            await userMgr.CreateAsync(user);

            //Add nhóm phân quyền
            var roleMgr = _ctx.GetService<RoleManager<IdentityRole>>();
            var role = await roleMgr.FindByIdAsync($"{model.GuidPrimissions}");

            await userMgr.AddToRoleAsync(user, role.Name);
            await userMgr.AddClaimAsync(user, new System.Security.Claims.Claim(nameof(user.GuidEmployee), user.GuidEmployee.ToString()));

            //Gửi mail tài khoản
            var token = await userMgr.GeneratePasswordResetTokenAsync(user);
            var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string subject = $"Thông tin tài khoản đăng nhập HRM";
            string url = $"{BaseUri}reset-password?token={validToken}&userid={user.UserName}";
            string content = $"<p>Chào bạn {user.LastName} {user.FirstName}<p>" +
                             $"<p>Hệ thống HRM xin thông báo:<p>" +
                             $"<p>Tài khoản truy cập Hệ thống của bạn đã được khởi tạo.<p>" +
                             $"<p>Tên đăng nhập của bạn là: <b>{model.UserName}</b><p>" +
                             $"<p>Vui lòng Click vào đây: <b><a href='{url}'>Tạo mật khẩu mới</a></b></p>";

            MailRequest mail = new MailRequest() {Subject = subject, ToEmail = user.Email, Content = content, Attachments = new()};

          //  _ = Task.Run(() => _svcMailSetting.SendMail(mail)); //await _svcMailSetting.SendMail(mail);

            return ResultOf<string>.Ok("OK");
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return ResultOf<string>.Ok("Đã có lỗi xảy ra!");
        }
    }

    public async Task<Result> ChangePassword(ModelPasswordChange model)
    {
        try
        {
            var userMgr = _ctx.GetService<UserManager<SA_USER>>();
            SA_USER user = await userMgr.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return Result.Error("Username không tồn tại!");
            }

            user.PasswordHash = userMgr.PasswordHasher.HashPassword(user, model.Password);
            var result = await userMgr.UpdateAsync(user);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return Result.Error("Đã có lỗi xảy ra!");
        }
    }

    public Task<bool> CheckPermissionAdmin()
    {
        return Task.FromResult(_ctx.CheckPermission(PERMISSION.ADMIN_ACCOUNTS));
    }
}
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;
using ESC.CONCOST.ModuleManagementCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestEase;
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
    public async Task<ResultsOf<ModelAccountListItem>> GetListAccounts()
    {
        try
        {
            var users = await _ctx.Set<SA_USER>().Where(x=>x.Id !="6f449fa3-3964-474b-b94b-efff192ef2ca")
                .Include(x => x.Customer)
                .AsNoTracking()
                .ToListAsync();

            var employees = await _ctx.Set<EntityEmployee>()
                .AsNoTracking()
                .ToListAsync();

            var result = new List<ModelAccountListItem>();

            // 1. Nhân sự nội bộ: lấy từ EntityEmployee, left join SA_USER
            foreach (var employee in employees)
            {
                var user = users.FirstOrDefault(x => x.GuidEmployee == employee.Guid);

                result.Add(new ModelAccountListItem
                {
                    AccountType = BasicCodes.AccountType.Employee,

                    UserId = user?.Id ?? string.Empty,
                    UserName = string.IsNullOrWhiteSpace(user?.UserName) ? "-" : user.UserName,
                    Email = employee.Email ?? user?.Email ?? string.Empty,
                    Phone = employee.Phone
                        ?? employee.Mobile1
                        ?? employee.Mobile2
                        ?? user?.PhoneNumber
                        ?? string.Empty,

                    Active = user == null ? null : user.Active,

                    GuidEmployee = employee.Guid,
                    EmployeeCode = employee.EmployeeCode ?? string.Empty,

                    FirstName = employee.FirstName ?? string.Empty,
                    LastName = employee.LastName ?? string.Empty,
                    DisplayName = employee.FullName
                });
            }

            // 2. Khách hàng ESC: lấy từ SA_USER có CustomerId
            var customerUsers = users
                .Where(x => x.CustomerId.HasValue)
                .ToList();

            foreach (var user in customerUsers)
            {
                var customer = user.Customer;

                result.Add(new ModelAccountListItem
                {
                    AccountType = BasicCodes.AccountType.Customer,

                    UserId = user.Id,
                    UserName = user.UserName ?? "-",
                    Email = user.Email ?? string.Empty,
                    Phone = user.PhoneNumber ?? string.Empty,
                    Active = user.Active,

                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,

                    CustomerId = user.CustomerId,
                    CustomerGuid = customer?.Guid,
                    CompanyName = customer?.CompanyName ?? string.Empty,
                    BusinessLicense = customer?.BusinessLicense ?? string.Empty,
                    CeoName = customer?.CeoName ?? string.Empty,
                    CustomerApprovalStatus = customer?.ApprovalStatus,
                    IsPaid = customer?.IsPaid,
                    MembershipType = customer?.MembershipType ?? string.Empty,
                    RequestDate = customer?.RequestDate,

                    DisplayName = !string.IsNullOrWhiteSpace(customer?.CompanyName)
                        ? customer.CompanyName
                        : $"{user.LastName} {user.FirstName}".Trim()
                });
            }

            // 3. Admin/System user: user không gắn employee và không gắn customer
            var adminUsers = users
                .Where(x => !x.CustomerId.HasValue && !x.GuidEmployee.HasValue)
                .ToList();

            foreach (var user in adminUsers)
            {
                var displayName = $"{user.LastName} {user.FirstName}".Trim();

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = user.UserName ?? "Admin";
                }

                result.Add(new ModelAccountListItem
                {
                    AccountType = BasicCodes.AccountType.Admin,

                    UserId = user.Id,
                    UserName = user.UserName ?? "-",
                    Email = user.Email ?? string.Empty,
                    Phone = user.PhoneNumber ?? string.Empty,
                    Active = user.Active,

                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    DisplayName = displayName
                });
            }

            var ordered = result
                .OrderBy(x => x.AccountType == BasicCodes.AccountType.Admin ? 0 : x.AccountType == BasicCodes.AccountType.Employee ? 1 : 2)
                .ThenBy(x => x.DisplayName)
                .ToList();

            return ResultsOf<ModelAccountListItem>.Ok(ordered);
        }
        catch (Exception ex)
        {
            _log.LogError($"{_ctx.Summary} - {ex.Message}");
            return ResultsOf<ModelAccountListItem>.Error("Đã có lỗi xảy ra khi tải danh sách tài khoản!");
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace ESC.CONCOST.WebHost.Pages;

public class ForgotPassword : PageModel
{
    private readonly UserManager<SA_USER> _userManager;
    private readonly IMailSettingService _svcMailSettings;
    public ForgotPassword(UserManager<SA_USER> userMgr, IMailSettingService svcMailSettings)
    {
        _userManager = userMgr;
        _svcMailSettings = svcMailSettings;
    }
    [BindProperty]
    public string Value { get; set; }
    [BindProperty]
    [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
    public string UserId { get; set; }

    #region snippet
    public async Task<IActionResult> OnGet([FromRoute] string value)
    {
        await Task.CompletedTask;
        Value = value;
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByNameAsync(UserId);

        if (user == null)
        {
            ModelState.AddModelError("UserId", "Tài khoản không tồn tại!");
            return Page();
        } 

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var validToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        string subject = $"Đổi mật khẩu đăng nhập HRM";
        string url = $"{Request.Scheme}://{Request.Host.Value}/reset-password?token={validToken}&userid={user.UserName}";
        string content = $"<p>Chào bạn {user.LastName} {user.FirstName}<p>";
        content += $"<p>Bạn vừa yêu cầu xác thực tài khoản/đặt lại mật khẩu.</p>";
        content += $"<p>Để đặt lại mật khẩu, vui lòng nhấn vào liên kết sau: <a href='{url}'>Đặt mật khẩu mới</a></p>";
        content += $"<p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>";

        MailRequest mail = new MailRequest() { Subject = subject, ToEmail = user.Email, Content = content, Attachments = new() };
        
        await _svcMailSettings.SendMail(mail);

        return Redirect("/forgot-password/success");
    }
    #endregion
}
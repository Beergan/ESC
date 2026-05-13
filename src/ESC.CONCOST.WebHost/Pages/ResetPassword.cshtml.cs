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

public class ResetPassword : PageModel
{
    private readonly UserManager<SA_USER> _userManager;
    public ResetPassword(UserManager<SA_USER> userMgr)
    {
        _userManager = userMgr;
    }
    public string UserId { get; set; }
    public string Token { get; set; }

    [BindProperty]
    public string Value { get; set; }

    [BindProperty]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Mật khẩu 8-15 ký tự, gồm hoa, thường, số, đặc biệt.")]
    [Required(ErrorMessage ="Mật khẩu không được để trống!")]
    public string Password { get; set; }

    [BindProperty]
    [Required(ErrorMessage ="Mật khẩu không được để trống!")]
    [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp!")]
    public string PasswordConfirm { get; set; }

    #region snippet
    public async Task<IActionResult> OnGet([FromRoute] string value)
    {
        await Task.CompletedTask;
        Value = value;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        UserId = Request.Query["userid"].ToString();
        Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Request.Query["token"].ToString()));

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByNameAsync(UserId);

        if (user == null)
        {
            ModelState.AddModelError("PasswordConfirm", "Tài khoản không tồn tại!");
            return Page();
        }  
        
        var reset = await _userManager.ResetPasswordAsync(user, Token, Password);
        if (!reset.Succeeded)
        {
            ModelState.AddModelError("PasswordConfirm", "Đã có lỗi xảy ra!");
            return Page();
        }

        return Redirect("/reset-password/success");
    }
    #endregion
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ESC.CONCOST.WebHost.Pages;

public class LoginModel : PageModel
{
    private readonly UserManager<SA_USER>  _userManager;
    private readonly IAuthService          _authSvc;
    private readonly IDbContext            _db;
    public ITextTranslator Text { get; set; }

    public LoginModel(
        IAuthService        svc,
        ITextTranslator     text,
        UserManager<SA_USER> userManager,
        IDbContext          db)
    {
        _authSvc     = svc;
        Text         = text;
        _userManager = userManager;
        _db          = db;
    }

    public IActionResult OnGet() => Page();

    [BindProperty] [Required] public string UserId   { get; set; }
    [BindProperty] [Required] public string Password { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", "이메일 또는 비밀번호를 입력해 주세요!");
            return Page();
        }

        // 1. Tìm user theo email (UserName = Email trong ESC)
        var user = await _userManager.FindByNameAsync(UserId)
                ?? await _userManager.FindByEmailAsync(UserId);

        if (user == null)
        {
            ModelState.AddModelError("error", "아이디 또는 비밀번호가 올바르지 않습니다.");
            return Page();
        }

        // 2. Kiểm tra tài khoản bị khóa (Active = false do Admin khóa thủ công)
        if (user.Active == false)
        {
            ModelState.AddModelError("error", "계정이 비활성화되었습니다. 관리자에게 문의해 주세요.");
            return Page();
        }

        // 3. Đăng nhập qua AuthService (xác thực password + tạo JWT cookie)
        var rsp = await _authSvc.Login(new LoginRequest
        {
            UserName = user.UserName,
            Password = Password
        });

        if (!rsp.Success)
        {
            ModelState.AddModelError("error", "아이디 또는 비밀번호가 올바르지 않습니다.");
            return Page();
        }

        // 4. Kiểm tra Customer approval status (nếu là ESC Customer)
        //    → Cho vào Home.razor, Home sẽ tự hiển thị banner "Đang chờ duyệt"
        //    → Không block login, chỉ set cookie thông báo để Home biết
        if (user.CustomerId.HasValue)
        {
            var customer = await _db.Set<Customer>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == user.CustomerId);

            if (customer != null && customer.ApprovalStatus == 0)
            {
                // Cho đăng nhập nhưng đánh dấu "pending" qua cookie
                Response.Cookies.Append("esc_pending", "1", new CookieOptions
                {
                    HttpOnly = false,   // Client JS cần đọc được
                    Expires  = System.DateTimeOffset.UtcNow.AddDays(1),
                    SameSite = SameSiteMode.Lax,
                });
            }
            else if (customer != null && customer.ApprovalStatus == 2)
            {
                // Bị từ chối — không cho vào
                ModelState.AddModelError("error",
                    $"계정이 거절되었습니다. 사유: {customer.RejectReason}");
                return Page();
            }
        }

        // 5. Redirect về Home
        HttpContext.Response.Cookies.Delete("blazorMode");
        HttpContext.Response.Cookies.Append("blazorMode", "server",
            new CookieOptions { Expires = System.DateTime.UtcNow.AddDays(30) });

        return Redirect("~/");
    }
}
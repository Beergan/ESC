using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ESC.CONCOST.WebHost.Pages;

public class EscRegisterModel : PageModel
{
    // Role ID cố định cho Customer / Khách hàng ESC
    private const string CUSTOMER_ROLE_ID = "c113ed87-ee13-4097-9ce1-b5bb24f28de4";

    private readonly UserManager<SA_USER>   _userMgr;
    private readonly RoleManager<IdentityRole> _roleMgr;
    private readonly IDbContext             _db;
    private readonly IMailSettingService    _mailSvc;

    // ── Bound Properties ──────────────────────────────────────────────────────
    [BindProperty] [Required] [StringLength(255)]
    public string CompanyName { get; set; } = "";

    [BindProperty] [Required] [StringLength(100)]
    public string CeoName { get; set; } = "";

    [BindProperty] [StringLength(50)]
    public string BusinessLicense { get; set; } = "";

    [BindProperty] [Required] [EmailAddress] [StringLength(255)]
    public string Email { get; set; } = "";

    [BindProperty] [Required] [MinLength(6)]
    public string Password { get; set; } = "";

    [BindProperty] [Required]
    public string PasswordConfirm { get; set; } = "";

    // Flag hiện trạng thái thành công
    public bool IsSuccess { get; private set; }

    // ── Constructor ───────────────────────────────────────────────────────────
    public EscRegisterModel(
        UserManager<SA_USER>      userMgr,
        RoleManager<IdentityRole> roleMgr,
        IDbContext                db,
        IMailSettingService       mailSvc)
    {
        _userMgr = userMgr;
        _roleMgr = roleMgr;
        _db      = db;
        _mailSvc = mailSvc;
    }

    public IActionResult OnGet() => Page();

    // ── POST Handler ──────────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Validate form
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("error", "모든 유효한 정보를 입력해주세요. / Please provide all valid information.");
            return Page();
        }

        if (Password != PasswordConfirm)
        {
            ModelState.AddModelError("error", "비밀번호가 일치하지 않습니다. / Passwords do not match.");
            return Page();
        }

        // 2. Kiểm tra email đã tồn tại chưa
        var emailExists = await _userMgr.FindByEmailAsync(Email);
        if (emailExists != null)
        {
            ModelState.AddModelError("error", "이미 등록된 이메일입니다. 로그인해주세요. / This email is already registered. Please sign in.");
            return Page();
        }
        using (var db = _db)
        {

            try
            {
                var customer = new Customer
                {
                    Guid = Guid.NewGuid(),          // Guid type
                    CompanyName = CompanyName.Trim(),
                    BusinessLicense = BusinessLicense?.Trim(),
                    CeoName = CeoName.Trim(),
                    ApprovalStatus = 0,        // Pending
                    MembershipType = "Free",
                    IsPaid = false,
                    RequestDate = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    UserCreated = Email,
                    UserModified = Email,
                    RejectReason = ""
                };
                await db.Repo<Customer>().Insert(customer);
                var user = new SA_USER
                {
                    UserName = Email,
                    Email = Email,
                    FirstName = CeoName.Trim(),
                    LastName = "",
                    CustomerId = customer.Id,       // Guid? from Customer.CustomerId (Guid)
                    Active = true,       // Cho đăng nhập ngay sau đăng ký
                    EmailConfirmed = true,
                    Note = "ESC Customer — Pending approval",
                    Logo = "",
                    ApprovedBy = "",         // NOT NULL constraint — empty until admin approves
                    ApprovedDate = null,       // Set when admin approves
                    Avatar = "",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedEmail = Email.ToUpperInvariant(),
                    NormalizedUserName = Email.ToUpperInvariant(),
                };
                user.PasswordHash = _userMgr.PasswordHasher.HashPassword(user, Password);
                // 3. Create User
                var result = await _userMgr.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                    ModelState.AddModelError("error", msg);
                    return Page();
                }

                // 4. Role
                var role = await _roleMgr.FindByIdAsync(CUSTOMER_ROLE_ID);
                if (role != null)
                    await _userMgr.AddToRoleAsync(user, role.Name);

                // 6. Email (sau commit)
                _ = Task.Run(() => SendWelcomeEmail(user, customer));

                IsSuccess = true;
                return Page();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", $"오류가 발생했습니다 / An error occurred: {ex.Message}");
                return Page();
            }
        }
    }

    // ── Email Helper ──────────────────────────────────────────────────────────
    private async Task SendWelcomeEmail(SA_USER user, Customer customer)
    {
        try
        {
            string loginUrl = $"{Request.Scheme}://{Request.Host}/login";
            string content =
                $"<p>Kính gửi <b>{customer.CeoName}</b>,</p>" +
                $"<p>Tài khoản ESC CON-COST của công ty <b>{customer.CompanyName}</b> đã được đăng ký thành công.</p>" +
                $"<p>Tài khoản đang chờ xét duyệt. Sau khi được phê duyệt, bạn có thể sử dụng đầy đủ tính năng hệ thống.</p>" +
                $"<p>Bạn có thể đăng nhập ngay tại: <a href='{loginUrl}'>{loginUrl}</a></p>" +
                $"<hr/><p style='color:#999; font-size:12px;'>ESC CON-COST — Construction Cost Adjustment System</p>";

            var mail = new MailRequest
            {
                Subject     = "ESC CON-COST — 회원가입 완료 안내 (등록 확인)",
                ToEmail     = user.Email,
                Content     = content,
                Attachments = new(),
            };

            await _mailSvc.SendMail(mail);
        }
        catch
        {
            // Email gửi thất bại không block đăng ký
        }
    }
}

using System.ComponentModel.DataAnnotations;
namespace ESC.CONCOST.ModuleManagementCore;
public class ModelPasswordChange
{
    public string UserName { get; set; }
    public string PasswordOld { get; set; }

    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Mật khẩu 8-15 ký tự, gồm hoa, thường, số, đặc biệt.")]
    [Required(ErrorMessage ="Mật khẩu không được để trống!")]
    public string Password { get; set; }

    [Required(ErrorMessage ="Mật khẩu không được để trống!")]
    [Compare("Password", ErrorMessage = "Xác nhận Mật khẩu không khớp!")]
    public string PasswordConfirm { get; set; }
}
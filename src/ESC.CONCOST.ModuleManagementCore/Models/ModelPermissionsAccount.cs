using System.ComponentModel.DataAnnotations;

namespace ESC.CONCOST.ModuleManagementCore;

public class ModelPermissionsAccount
{
    public string UserId { get; set; }

    [Display(Name = "Tên người dùng")]
    public string UserName { get; set; }
    public string RoleId { get; set; }

    [Display(Name = "Họ tên")]
    public string FullName { get; set; }

    [Display(Name = "Tên")]
    public string FirstName { get; set; }

    [Display(Name = "Họ đệm")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }
    
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; }

    public string Avatar { get; set;}
    
}
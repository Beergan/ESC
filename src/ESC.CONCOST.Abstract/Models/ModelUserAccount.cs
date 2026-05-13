
using System;
using System.ComponentModel.DataAnnotations;

namespace ESC.CONCOST.Abstract;

public class ModelUserAccount
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Mật khẩu 8-15 ký tự, gồm hoa, thường, số, đặc biệt.")]
    public string Password { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string PhoneCountry { get; set; } = "vn";

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Avatar { get; set; } = string.Empty;

    public bool EmployeeConnected { get; set; }

    public Guid? GuidEmployee { get; set; }

    [RequiredGuid(ErrorMessage = "Nhóm phân quyền không được để trống!")]
    public Guid GuidPrimissions { get; set; }
}
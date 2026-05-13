using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using ESC.CONCOST.Abstract;

[Table("EMPLOYEE")]
public class EntityEmployee : EntityBase
{
    // Mã nhân viên (VD: "VQS-001", "CCS-042")
    [Display(Name = "Mã nhân viên|사번")]
    public string? EmployeeCode { get; set; }

    // === Thông tin cá nhân ===
    [Display(Name = "Ảnh đại diện|프로필 사진")]
    public string? Avatar { get; set; }

    [Required(ErrorMessage = "Họ đệm không được để trống!|성(姓)은 필수 항목입니다!")]
    [Display(Name = "Họ đệm|성")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Tên không được để trống!|이름은 필수 항목입니다!")]
    [Display(Name = "Tên|이름")]
    public string FirstName { get; set; }

    [NotMapped]
    [Display(Name = "Họ và tên|성명")]
    public string FullName => $"{LastName} {FirstName}";

    [Required(ErrorMessage = "Email không được để trống!|이메일은 필수 항목입니다!")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ!|이메일 형식이 올바르지 않습니다!")]
    [Display(Name = "Email|이메일")]
    public string Email { get; set; }

    [Display(Name = "Số CCCD|주민등록번호")]
    public string? CitizenID { get; set; }

    [Display(Name = "Số điện thoại nội bộ|내선전화")]
    public string? Phone { get; set; }

    [Display(Name = "Di động 1|핸드폰1")]
    public string? Mobile1 { get; set; }

    [Display(Name = "Di động 2|핸드폰2")]
    public string? Mobile2 { get; set; }

    [Display(Name = "Điện thoại Internet|인터넷전화")]
    public string? Voip { get; set; }

    [Display(Name = "Số Fax|팩스번호")]
    public string? Fax { get; set; }

    // === Địa chỉ ===
    [Display(Name = "Mã bưu điện|우편번호")]
    public string? ZipCode { get; set; }

    [Display(Name = "Địa chỉ|주소")]
    public string? Address { get; set; }

    [Display(Name = "Địa chỉ chi tiết|상세주소")]
    public string? AddressDetail { get; set; }

    [Display(Name = "Ngày sinh|생년월일")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính|성별")]
    public string? Gender { get; set; }

    // === Ngày tháng ===
    [Display(Name = "Ngày vào làm|입사일")]
    public DateTime? HireDate { get; set; }

    [Display(Name = "Ngày nghỉ việc|퇴사일")]
    public DateTime? LeaveDate { get; set; }

    // === Chuyên môn & Nhiệm vụ ===
    [Display(Name = "Nhiệm vụ chính|담당업무")]
    public string? Responsibilities { get; set; }

    [Display(Name = "Trình độ học vấn|학력")]
    public string? Education_Level { get; set; }

    [Display(Name = "Chuyên môn|전공")]
    public string? ProfessionalQualification { get; set; }

    public string? LangId { get; set; }

    [Display(Name = "Kích hoạt|활성화")]
    public bool Active { get; set; } = true;
}
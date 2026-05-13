using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleEmployeeCore;

[Feature(Name = "ModuleEmployee", TextEn = "Employee Module", TextVi = "MODULE NHÂN SỰ", TextKo = "인사 모듈")]
public enum PERMISSION
{
    [Function(TextEn = "View employee list", TextVi = "Xem danh sách nhân sự", TextKo = "인사 목록 조회")]
    EMPLOYEE_VIEW,

    [Function(TextEn = "View employee profile", TextVi = "Xem hồ sơ nhân sự", TextKo = "인사 기록 조회")]
    FILE_EMPLOYEE_VIEW,

    [Function(TextEn = "Create/Update employee", TextVi = "Tạo mới/ hiệu chỉnh nhân sự", TextKo = "인사 정보 등록/수정")]
    EMPLOYEE_CREATE_UPDATE,

    [Function(TextEn = "Activate employee account", TextVi = "Kích hoạt tài khoản nhân sự", TextKo = "인사 계정 활성화")]
    EMPLOYEE_ACTIVE_ACCOUNT,

    [Function(TextEn = "Personal documents", TextVi = "Tài liệu cá nhân", TextKo = "개인 문서")]
    EMPLOYEE_DOCUMENT,
}
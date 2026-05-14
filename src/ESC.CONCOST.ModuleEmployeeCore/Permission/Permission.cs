using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleEmployeeCore;

[Feature(Name = "ModuleEmployee", TextKo = "인사 모듈", TextEn = "Employee Module")]
public enum PERMISSION
{
    [Function(TextKo = "인사 목록 조회", TextEn = "View employee list")]
    EMPLOYEE_VIEW,

    [Function(TextKo = "인사 기록 조회", TextEn = "View employee profile")]
    FILE_EMPLOYEE_VIEW,

    [Function(TextKo = "인사 정보 등록/수정", TextEn = "Create/Update employee")]
    EMPLOYEE_CREATE_UPDATE,

    [Function(TextKo = "인사 계정 활성화", TextEn = "Activate employee account")]
    EMPLOYEE_ACTIVE_ACCOUNT,

    [Function(TextKo = "개인 문서", TextEn = "Personal documents")]
    EMPLOYEE_DOCUMENT,
}
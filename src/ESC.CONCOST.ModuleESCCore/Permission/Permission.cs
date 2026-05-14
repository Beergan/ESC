using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleESCCore;

[Feature(Name = "ModuleESC", TextKo = "ESC 조정 모듈", TextEn = "ESC Module")]
public enum PERMISSION
{
    [Function(TextKo = "ESC 프로젝트 목록 조회", TextEn = "View ESC workspace")]
    ESC_WORKSPACE_VIEW,

    [Function(TextKo = "모든 ESC 프로젝트 조회", TextEn = "View all ESC projects")]
    ESC_WORKSPACE_VIEW_ALL,

    [Function(TextKo = "ESC 프로젝트 등록/수정", TextEn = "Create/Update ESC project")]
    ESC_PROJECT_CREATE_UPDATE,

    [Function(TextKo = "ESC 프로젝트 삭제", TextEn = "Delete ESC project")]
    ESC_PROJECT_DELETE,

    [Function(TextKo = "ESC 고객 목록 조회", TextEn = "View Customers ESC")]
    ESC_CUSTOMER_VIEW,
    [Function(TextKo = "ESC 고객 승인/거절", TextEn = "Approve / Reject Customers ESC")]
    ESC_CUSTOMER_APPROVE_REJECT,
}
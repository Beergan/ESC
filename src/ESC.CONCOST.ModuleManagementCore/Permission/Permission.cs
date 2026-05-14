using System;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleManagementCore;

[Feature(Name = "ModuleManagement", TextKo = "관리 모듈", TextEn = "Management Module")]
public enum PERMISSION
{
    [Function(TextKo = "계정 관리", TextEn = "Manage accounts")]
    ADMIN_ACCOUNTS,

    [Function(TextKo = "시스템 로그 조회", TextEn = "View system logs")]
    AUDIT_LOG,
}
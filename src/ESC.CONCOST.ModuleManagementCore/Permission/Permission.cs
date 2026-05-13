using System;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleManagementCore;

[Feature(Name = "ModuleManagement", TextEn = "Management Module", TextVi = "MODULE QUẢN TRỊ", TextKo = "관리 모듈")]
public enum PERMISSION
{
    [Function(TextEn = "Manage accounts", TextVi = "Quản trị", TextKo = "계정 관리")]
    ADMIN_ACCOUNTS,

    [Function(TextEn = "View system logs", TextVi = "Xem logs hệ thống", TextKo = "시스템 로그 조회")]
    AUDIT_LOG,
}
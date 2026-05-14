using System;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleSettingCore;

[Feature(Name = "ModuleSetting", TextKo = "설정 모듈", TextEn = "Setting Module")]
public enum PERMISSION
{
    [Function(TextKo = "지수 DB 관리", TextEn = "Manage ESC Index DB")]
    ESC_INDEX_DB_MANAGE,
            
    [Function(TextKo = "계산식 설정", TextEn = "Manage ESC Formula Settings")]
    ESC_FORMULA_SETTING_MANAGE,

    [Function(TextKo = "보고서 표시 설정", TextEn = "Manage ESC Report Visibility")]
    ESC_REPORT_VISIBILITY_MANAGE,
}
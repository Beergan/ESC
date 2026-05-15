using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System.Collections.Generic;

namespace ESC.CONCOST.ModuleSetting;

/// <summary>
/// Mapper tập trung toàn bộ logic ánh xạ field từ model vào entity.
/// Không chứa logic nghiệp vụ — chỉ copy và sanitize giá trị.
/// </summary>
public static class EscFormulaMapper
{
    /// <summary>
    /// Map toàn bộ field từ model sang entity (không đụng đến Guid, DateCreated, DateModified).
    /// </summary>
    public static void Map(EscFormulaSetting model, EscFormulaSetting entity)
    {
        // Identity
        entity.FormulaCode = model.FormulaCode.SafeTrim();
        entity.FormulaName = model.FormulaName.SafeTrim();
        entity.FormulaNameEn = model.FormulaNameEn.SafeTrim();
        entity.VersionNo = model.VersionNo <= 0 ? 1 : model.VersionNo;

        // Formulas
        entity.WeightFormula = model.WeightFormula.SafeTrim();
        entity.IndexRatioFormula = model.IndexRatioFormula.SafeTrim();
        entity.WeightedRatioFormula = model.WeightedRatioFormula.SafeTrim();
        entity.CompositeFormula = model.CompositeFormula.SafeTrim();
        entity.AdjustmentRateFormula = model.AdjustmentRateFormula.SafeTrim();
        entity.ApplicableAmountFormula = model.ApplicableAmountFormula.SafeTrim();
        entity.GrossAdjustmentFormula = model.GrossAdjustmentFormula.SafeTrim();
        entity.AdvanceDeductionFormula = model.AdvanceDeductionFormula.SafeTrim();
        entity.FinalAdjustmentFormula = model.FinalAdjustmentFormula.SafeTrim();
        entity.EligibleConditionFormula = model.EligibleConditionFormula.SafeTrim();

        // Config
        entity.ThresholdRate = model.ThresholdRate <= 0 ? 3m : model.ThresholdRate;
        entity.ThresholdDays = model.ThresholdDays <= 0 ? 90 : model.ThresholdDays;
        entity.RoundingMethod = string.IsNullOrWhiteSpace(model.RoundingMethod)
            ? BasicCodes.EscFormulaRounding.Round
            : model.RoundingMethod.Trim();
        entity.DecimalPlaces = model.DecimalPlaces < 0 ? 0 : model.DecimalPlaces;
        entity.UseAdvanceDeduction = model.UseAdvanceDeduction;
        entity.OtherDeductionDefault = model.OtherDeductionDefault < 0 ? 0 : model.OtherDeductionDefault;

        // Flags
        entity.IsDefault = model.IsDefault;
        entity.IsActive = model.IsCurrent ? true : model.IsActive; // IsCurrent luôn kéo IsActive = true
        entity.IsCurrent = model.IsCurrent;
        entity.SortOrder = model.SortOrder;
        entity.Description = model.Description.SafeTrim();
    }

    /// <summary>
    /// Trả về map công thức cần validate/evaluate.
    /// Key = tên field (dùng trong error message), Value = giá trị công thức.
    /// </summary>
    public static Dictionary<string, string> GetFormulaMap(EscFormulaSetting model) => new()
    {
        ["WeightFormula"] = model.WeightFormula,
        ["IndexRatioFormula"] = model.IndexRatioFormula,
        ["WeightedRatioFormula"] = model.WeightedRatioFormula,
        ["CompositeFormula"] = model.CompositeFormula,
        ["AdjustmentRateFormula"] = model.AdjustmentRateFormula,
        ["ApplicableAmountFormula"] = model.ApplicableAmountFormula,
        ["GrossAdjustmentFormula"] = model.GrossAdjustmentFormula,
        ["AdvanceDeductionFormula"] = model.AdvanceDeductionFormula,
        ["FinalAdjustmentFormula"] = model.FinalAdjustmentFormula,
        ["EligibleConditionFormula"] = model.EligibleConditionFormula
    };
}

/// <summary>Extension methods tiện ích dùng nội bộ.</summary>
internal static class StringExtensions
{
    public static string SafeTrim(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    /// <summary>Trả về fallback nếu chuỗi rỗng/null.</summary>
    public static string ToAuditReason(this string? value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}
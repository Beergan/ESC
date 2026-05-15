using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ESC.CONCOST.ModuleSetting;

// ═══════════════════════════════════════════════════════════
// FormulaEvaluationContext
// Đóng gói toàn bộ biến và thứ tự tính toán công thức.
// Tách ra để:
//   1. Enforce dependency order (Weight → IndexRatio → ... → Final)
//   2. Dễ unit test từng bước độc lập
//   3. TestFormulaAsync gọn hơn — không còn mutation dict rải rác
// ═══════════════════════════════════════════════════════════
public class FormulaEvaluationContext
{
    // Thứ tự khai báo = thứ tự tính toán — KHÔNG được đổi
    // Step 1 → Step 9
    private readonly Dictionary<string, decimal> _vars;

    private FormulaEvaluationContext(Dictionary<string, decimal> baseVars)
    {
        _vars = new Dictionary<string, decimal>(baseVars, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Khởi tạo context từ input của user và cấu hình model.</summary>
    public static FormulaEvaluationContext FromInput(EscFormulaSetting model, FormulaTestInputDto input) =>
        new(new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            // --- Input từ user ---
            ["Amount"] = input.Amount,
            ["TotalCost"] = input.TotalCost,
            ["BaseIndex"] = input.BaseIndex,
            ["CompareIndex"] = input.CompareIndex,
            ["ContractAmount"] = input.ContractAmount,
            ["ExcludedAmount"] = input.ExcludedAmount,
            ["AdvanceAmount"] = input.AdvanceAmount,
            ["OtherDeduction"] = input.OtherDeduction,
            ["ElapsedDays"] = input.ElapsedDays,
            // --- Config từ model ---
            ["ThresholdRate"] = model.ThresholdRate,
            ["ThresholdDays"] = model.ThresholdDays
        });

    /// <summary>
    /// Tính toán tuần tự theo đúng dependency chain.
    /// Thứ tự bắt buộc — xem comment từng Step.
    /// </summary>
    public FormulaTestResultDto Evaluate(EscFormulaSetting model)
    {
        var result = new FormulaTestResultDto();

        // Step 1: Weight = Amount / TotalCost
        result.Weight = Eval(model.WeightFormula);
        Set("Weight", result.Weight);

        // Step 2: IndexRatio = CompareIndex / BaseIndex — cần BaseIndex, CompareIndex
        result.IndexRatio = Eval(model.IndexRatioFormula);
        Set("IndexRatio", result.IndexRatio);

        // Step 3: WeightedRatio = Weight * IndexRatio — cần Weight, IndexRatio
        result.WeightedRatio = Eval(model.WeightedRatioFormula);
        Set("WeightedRatio", result.WeightedRatio);

        // Step 4: CompositeCoefficient = SUM(WeightedRatio) hoặc custom formula
        result.CompositeCoefficient = EvalComposite(model.CompositeFormula, result.WeightedRatio);
        Set("CompositeCoefficient", result.CompositeCoefficient);

        // Step 5: AdjustmentRate = (CompositeCoefficient - 1) * 100 — cần CompositeCoefficient
        result.AdjustmentRate = Eval(model.AdjustmentRateFormula);
        Set("AdjustmentRate", result.AdjustmentRate);

        // Step 6: ApplicableAmount = ContractAmount - ExcludedAmount
        result.ApplicableAmount = Eval(model.ApplicableAmountFormula);
        Set("ApplicableAmount", result.ApplicableAmount);

        // Step 7: GrossAdjustmentAmount = AdjustmentRate * ApplicableAmount — cần cả 2 trên
        result.GrossAdjustmentAmount = Eval(model.GrossAdjustmentFormula);
        Set("GrossAdjustmentAmount", result.GrossAdjustmentAmount);

        // Step 8: AdvanceDeduction (optional) — cần GrossAdjustmentAmount, AdvanceAmount, ContractAmount
        result.AdvanceDeduction = model.UseAdvanceDeduction
            ? Eval(model.AdvanceDeductionFormula)
            : 0m;
        Set("AdvanceDeduction", result.AdvanceDeduction);

        // Step 9: FinalAdjustmentAmount = GrossAdjustmentAmount - AdvanceDeduction - OtherDeduction
        var raw = Eval(model.FinalAdjustmentFormula);
        result.FinalAdjustmentAmount = EscFormulaRounding.Apply(raw, model.RoundingMethod, model.DecimalPlaces);
        Set("FinalAdjustmentAmount", result.FinalAdjustmentAmount);

        // Step 10: Eligible condition — cần AdjustmentRate, ElapsedDays, ThresholdRate, ThresholdDays
        result.IsEligible = FormulaExpressionEngine.EvaluateCondition(model.EligibleConditionFormula, _vars);

        result.Variables = new Dictionary<string, decimal>(_vars);

        return result;
    }

    private decimal Eval(string formula) =>
        FormulaExpressionEngine.EvaluateDecimal(formula, _vars);

    private void Set(string key, decimal value) =>
        _vars[key] = value;

    private decimal EvalComposite(string formula, decimal weightedRatio)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return weightedRatio;

        if (formula.Trim().Equals("SUM(WeightedRatio)", StringComparison.OrdinalIgnoreCase))
            return weightedRatio;

        // Custom composite formula dùng SumWeightedRatio thay vì SUM(...)
        Set("SumWeightedRatio", weightedRatio);
        return Eval(formula);
    }
}

// ═══════════════════════════════════════════════════════════
// EscFormulaRounding
// Tách logic làm tròn ra static class riêng — dễ test, dễ mở rộng
// ═══════════════════════════════════════════════════════════
public static class EscFormulaRounding
{
    /// <summary>
    /// Áp dụng phương thức làm tròn.
    /// Mặc định: Round (AwayFromZero).
    /// </summary>
    public static decimal Apply(decimal value, string? roundingMethod, int decimalPlaces)
    {
        var method = string.IsNullOrWhiteSpace(roundingMethod)
            ? BasicCodes.EscFormulaRounding.Round
            : roundingMethod.Trim().ToUpperInvariant();

        return method switch
        {
            "FLOOR" => Math.Floor(value),
            "CEILING" => Math.Ceiling(value),
            "NONE" => value,
            _ => Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero)
        };
    }
}

// ═══════════════════════════════════════════════════════════
// EscFormulaHistoryHelper
// Tách logic tạo audit history — không còn async Task giả
// ═══════════════════════════════════════════════════════════
public static class EscFormulaHistoryHelper
{
    /// <summary>
    /// Thêm bản ghi lịch sử vào context (chưa SaveChanges).
    /// Nên gọi trước khi modify entity, để snapshot đúng trạng thái cũ.
    /// </summary>
    public static void Append(
        IDbContext db,
        EscFormulaSetting entity,
        string changeNote,
        DateTime now)
    {
        var history = new EscFormulaHistory
        {
            Guid = Guid.NewGuid(),
            FormulaSettingId = entity.Id,
            FormulaSettingGuid = entity.Guid,
            VersionNo = entity.VersionNo,
            SnapshotJson = JsonConvert.SerializeObject(entity),
            ChangeNote = string.IsNullOrWhiteSpace(changeNote) ? string.Empty : changeNote.Trim(),
            DateCreated = now,
            DateModified = now
        };

        db.Set<EscFormulaHistory>().Add(history);
    }
}

// ═══════════════════════════════════════════════════════════
// EscFormulaDefaultVariables
// Tách danh sách biến mặc định ra class riêng — Single Responsibility
// ═══════════════════════════════════════════════════════════
public static class EscFormulaDefaultVariables
{
    /// <summary>
    /// Trả về danh sách biến hệ thống mặc định khi DB chưa có dữ liệu.
    /// </summary>
    public static List<EscFormulaVariable> Create() =>
    [
        New("Amount",                "비목 금액",          "Cost item amount",          "비목/그룹의 금액",                                1),
        New("TotalCost",             "전체 금액 합계",      "Total cost amount",         "전체 비목 금액 합계",                              2),
        New("Weight",                "가중치",             "Weight",                    "Amount / TotalCost",                              3),
        New("BaseIndex",             "기준월 지수",         "Base index",                "기준월 지수",                                     4),
        New("CompareIndex",          "비교월 지수",         "Compare index",             "비교월 지수",                                     5),
        New("IndexRatio",            "지수비율",            "Index ratio",               "CompareIndex / BaseIndex",                        6),
        New("WeightedRatio",         "가중 지수비율",       "Weighted ratio",            "Weight * IndexRatio",                             7),
        New("CompositeCoefficient",  "종합계수",            "Composite coefficient",     "SUM(WeightedRatio)",                              8),
        New("AdjustmentRate",        "등락율",              "Adjustment rate",           "(CompositeCoefficient - 1) * 100",                9),
        New("ContractAmount",        "계약금액",            "Contract amount",           "계약금액",                                       10),
        New("ExcludedAmount",        "적용 제외 금액",      "Excluded amount",           "적용 제외 금액",                                 11),
        New("ApplicableAmount",      "적용대가",            "Applicable amount",         "ContractAmount - ExcludedAmount",                12),
        New("GrossAdjustmentAmount", "조정금액",            "Gross adjustment amount",   "선금공제 전 조정금액",                            13),
        New("AdvanceAmount",         "선금액",              "Advance amount",            "선금액",                                         14),
        New("AdvanceDeduction",      "선금공제액",          "Advance deduction",         "AdvanceAmount / ContractAmount * GrossAdjustmentAmount", 15),
        New("OtherDeduction",        "기타 공제액",         "Other deduction",           "기타 공제액",                                    16),
        New("FinalAdjustmentAmount", "최종 조정금액",       "Final adjustment amount",   "최종 조정금액",                                  17),
        New("ElapsedDays",           "경과일수",            "Elapsed days",              "기준일부터 조정일까지 경과일수",                  18),
        New("ThresholdRate",         "기준 등락율",         "Threshold rate",            "기본 3%",                                        19),
        New("ThresholdDays",         "기준 경과일수",       "Threshold days",            "기본 90일",                                      20)
    ];

    private static EscFormulaVariable New(
        string code, string nameKr, string nameEn, string description, int sortOrder) => new()
        {
            Guid = Guid.NewGuid(),
            VariableCode = code,
            VariableName = nameKr,
            VariableNameEn = nameEn,
            DataType = BasicCodes.EscFormulaFieldType.Decimal,
            DefaultValue = string.Empty,
            Description = description,
            IsSystem = true,
            IsActive = true,
            SortOrder = sortOrder
        };
}
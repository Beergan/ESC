using System;
using System.Collections.Generic;

namespace ESC.CONCOST.Abstract;

public class FormulaVariableViewDto
{
    public string Code { get; set; } = string.Empty;
    public string NameKr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string DataType { get; set; } = BasicCodes.EscFormulaFieldType.Decimal;
    public string Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; } = true;
    public int SortOrder { get; set; }
}

public class FormulaTestInputDto
{
    public decimal Amount { get; set; } = 1_500_000m;
    public decimal TotalCost { get; set; } = 10_000_000m;
    public decimal BaseIndex { get; set; } = 100m;
    public decimal CompareIndex { get; set; } = 105.5m;

    public decimal ContractAmount { get; set; } = 12_000_000m;
    public decimal ExcludedAmount { get; set; } = 500_000m;
    public decimal AdvanceAmount { get; set; } = 1_000_000m;
    public decimal OtherDeduction { get; set; } = 0m;

    public decimal ElapsedDays { get; set; } = 120m;
}

public class FormulaTestResultDto
{
    public decimal Weight { get; set; }
    public decimal IndexRatio { get; set; }
    public decimal WeightedRatio { get; set; }
    public decimal CompositeCoefficient { get; set; }
    public decimal AdjustmentRate { get; set; }
    public decimal ApplicableAmount { get; set; }
    public decimal GrossAdjustmentAmount { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal FinalAdjustmentAmount { get; set; }
    public bool IsEligible { get; set; }

    public Dictionary<string, decimal> Variables { get; set; } = new();
}
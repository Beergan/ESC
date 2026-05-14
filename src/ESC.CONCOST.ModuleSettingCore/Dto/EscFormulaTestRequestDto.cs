namespace ESC.CONCOST.ModuleSettingCore;

public class EscFormulaTestRequestDto
{
    public string FormulaExpression { get; set; } = string.Empty;

    public decimal ContractAmount { get; set; } = 0;

    public decimal ExcludedAmount { get; set; }

    public decimal AdjustmentRate { get; set; } = 3.0m;

    public decimal AdvanceAmount { get; set; }

    public decimal OtherDeduction { get; set; }

    public string RoundingMethod { get; set; } = "ROUND";
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_FORMULA_SETTINGS")]
    public class EscFormulaSetting : EntityBase
    {
        [Column("formula_code")]
        [Required]
        [StringLength(100)]
        public string FormulaCode { get; set; } = string.Empty;

        [Column("formula_name")]
        [Required]
        [StringLength(255)]
        public string FormulaName { get; set; } = string.Empty;

        [Column("formula_name_en")]
        [StringLength(255)]
        public string FormulaNameEn { get; set; } = string.Empty;

        [Column("version_no")]
        public int VersionNo { get; set; } = 1;

        [Column("weight_formula")]
        [StringLength(1000)]
        public string WeightFormula { get; set; } = string.Empty;

        [Column("index_ratio_formula")]
        [StringLength(1000)]
        public string IndexRatioFormula { get; set; } = string.Empty;

        [Column("weighted_ratio_formula")]
        [StringLength(1000)]
        public string WeightedRatioFormula { get; set; } = string.Empty;

        [Column("composite_formula")]
        [StringLength(1000)]
        public string CompositeFormula { get; set; } = string.Empty;

        [Column("adjustment_rate_formula")]
        [StringLength(1000)]
        public string AdjustmentRateFormula { get; set; } = string.Empty;

        [Column("applicable_amount_formula")]
        [StringLength(1000)]
        public string ApplicableAmountFormula { get; set; } = string.Empty;

        [Column("gross_adjustment_formula")]
        [StringLength(1000)]
        public string GrossAdjustmentFormula { get; set; } = string.Empty;

        [Column("advance_deduct_formula")]
        [StringLength(1000)]
        public string AdvanceDeductionFormula { get; set; } = string.Empty;

        [Column("final_adjustment_formula")]
        [StringLength(1000)]
        public string FinalAdjustmentFormula { get; set; } = string.Empty;

        [Column("eligible_condition_formula")]
        [StringLength(1000)]
        public string EligibleConditionFormula { get; set; } = string.Empty;

        [Column("threshold_rate")]
        public decimal ThresholdRate { get; set; } = 3.0m;

        [Column("threshold_days")]
        public int ThresholdDays { get; set; } = 90;

        [Column("rounding_method")]
        [Required]
        [StringLength(30)]
        public string RoundingMethod { get; set; } = BasicCodes.EscFormulaRounding.Round;

        [Column("decimal_places")]
        public int DecimalPlaces { get; set; } = 0;

        [Column("use_advance_deduction")]
        public bool UseAdvanceDeduction { get; set; } = true;

        [Column("other_deduction_default")]
        public long OtherDeductionDefault { get; set; } = 0;

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_current")]
        public bool IsCurrent { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
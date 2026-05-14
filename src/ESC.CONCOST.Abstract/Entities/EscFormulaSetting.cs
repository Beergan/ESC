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

        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column("formula_expression")]
        [Required]
        [StringLength(4000)]
        public string FormulaExpression { get; set; } = string.Empty;

        [Column("rounding_method")]
        [Required]
        [StringLength(30)]
        public string RoundingMethod { get; set; } = BasicCodes.EscFormulaRounding.Round;

        [Column("decimal_places")]
        public int DecimalPlaces { get; set; } = 0;

        [Column("allow_negative_result")]
        public bool AllowNegativeResult { get; set; } = true;

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        public virtual ICollection<EscFormulaField> Fields { get; set; } = new List<EscFormulaField>();
    }
}
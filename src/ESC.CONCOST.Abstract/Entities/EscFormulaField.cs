using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_FORMULA_FIELDS")]
    public class EscFormulaField : EntityBase
    {
        [Column("formula_setting_id")]
        [Required]
        public int FormulaSettingId { get; set; }

        [ForeignKey(nameof(FormulaSettingId))]
        public virtual EscFormulaSetting? FormulaSetting { get; set; }

        [Column("field_key")]
        [Required]
        [StringLength(100)]
        public string FieldKey { get; set; } = string.Empty;

        [Column("label_ko")]
        [Required]
        [StringLength(255)]
        public string LabelKo { get; set; } = string.Empty;

        [Column("label_en")]
        [StringLength(255)]
        public string LabelEn { get; set; } = string.Empty;

        [Column("field_type")]
        [Required]
        [StringLength(50)]
        public string FieldType { get; set; } = BasicCodes.EscFormulaFieldType.Decimal;

        [Column("default_value")]
        [StringLength(500)]
        public string DefaultValue { get; set; } = string.Empty;

        [Column("placeholder")]
        [StringLength(255)]
        public string Placeholder { get; set; } = string.Empty;

        [Column("unit")]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Column("is_required")]
        public bool IsRequired { get; set; } = true;

        [Column("is_readonly")]
        public bool IsReadonly { get; set; }

        [Column("use_in_formula")]
        public bool UseInFormula { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("validation_min", TypeName = "decimal(18,6)")]
        public decimal? ValidationMin { get; set; }

        [Column("validation_max", TypeName = "decimal(18,6)")]
        public decimal? ValidationMax { get; set; }

        public virtual ICollection<EscFormulaFieldOption> Options { get; set; } = new List<EscFormulaFieldOption>();
    }
}
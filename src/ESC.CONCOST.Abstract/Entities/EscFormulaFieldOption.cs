using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_FORMULA_FIELD_OPTIONS")]
    public class EscFormulaFieldOption : EntityBase
    {
        [Column("formula_field_id")]
        [Required]
        public int FormulaFieldId { get; set; }

        [ForeignKey(nameof(FormulaFieldId))]
        public virtual EscFormulaField? FormulaField { get; set; }

        [Column("option_value")]
        [Required]
        [StringLength(255)]
        public string OptionValue { get; set; } = string.Empty;

        [Column("option_text_ko")]
        [Required]
        [StringLength(255)]
        public string OptionTextKo { get; set; } = string.Empty;

        [Column("option_text_en")]
        [StringLength(255)]
        public string OptionTextEn { get; set; } = string.Empty;

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
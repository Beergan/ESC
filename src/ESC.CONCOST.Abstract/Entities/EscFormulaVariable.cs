using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_FORMULA_VARIABLES")]
    public class EscFormulaVariable : EntityBase
    {
        [Column("variable_code")]
        [Required]
        [StringLength(100)]
        public string VariableCode { get; set; } = string.Empty;

        [Column("variable_name")]
        [Required]
        [StringLength(255)]
        public string VariableName { get; set; } = string.Empty;

        [Column("variable_name_en")]
        [StringLength(255)]
        public string VariableNameEn { get; set; } = string.Empty;

        [Column("data_type")]
        [StringLength(50)]
        public string DataType { get; set; } = BasicCodes.EscFormulaFieldType.Decimal;

        [Column("default_value")]
        [StringLength(500)]
        public string DefaultValue { get; set; } = string.Empty;

        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column("is_system")]
        public bool IsSystem { get; set; } = true;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; }
    }
}

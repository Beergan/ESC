using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_FORMULA_HISTORY")]
    public class EscFormulaHistory : EntityBase
    {
        [Column("formula_setting_id")]
        [Required]
        public int FormulaSettingId { get; set; }

        [Column("formula_setting_guid")]
        public Guid FormulaSettingGuid { get; set; }

        [Column("version_no")]
        public int VersionNo { get; set; }

        [Column("snapshot_json")]
        [Required]
        public string SnapshotJson { get; set; } = string.Empty;

        [Column("change_note")]
        [StringLength(1000)]
        public string ChangeNote { get; set; } = string.Empty;

        [ForeignKey(nameof(FormulaSettingId))]
        public virtual EscFormulaSetting FormulaSetting { get; set; }
    }
}

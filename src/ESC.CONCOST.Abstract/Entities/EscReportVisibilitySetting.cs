using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_REPORT_VISIBILITY_SETTINGS")]
    public class EscReportVisibilitySetting : EntityBase
    {
        [Column("permission_id")]
        [Required]
        public int PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual EntityPermission? Permission { get; set; }

        [Column("box_code")]
        [Required]
        [StringLength(100)]
        public string BoxCode { get; set; } = string.Empty;

        [Column("is_visible")]
        public bool IsVisible { get; set; } = true;
    }
}
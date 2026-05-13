using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ADMIN_SETTINGS")]
    public class AdminSetting: EntityBase
    {
        [Column("setting_value")]
        public string SettingValue { get; set; }

        [Column("description")]
        [StringLength(255)]
        public string Description { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("SETTING_PERMISSION")]
    public class SettingPermission : EntityBase
    {

        [Column("role_name")]
        [StringLength(100)]
        public string RoleName { get; set; }

        [Column("permission_key")]
        [StringLength(100)]
        public string PermissionKey { get; set; }

        [Column("is_allowed")]
        public bool IsAllowed { get; set; }
    }
}

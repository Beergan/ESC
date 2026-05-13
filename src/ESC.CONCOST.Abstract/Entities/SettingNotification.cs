using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("SETTING_NOTIFICATION")]
    public class SettingNotification : EntityBase
    {

        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("target_user_guids")]
        public string TargetUserGuids { get; set; }
    }
}

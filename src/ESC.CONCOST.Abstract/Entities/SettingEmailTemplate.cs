using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("SETTING_EMAIL_TEMPLATE")]
    public class SettingEmailTemplate : EntityBase
    {
        [Column("template_name")]
        [Required]
        [StringLength(255)]
        public string TemplateName { get; set; }

        [Column("subject_template")]
        [StringLength(500)]
        public string SubjectTemplate { get; set; }

        [Column("body_template")]
        public string BodyTemplate { get; set; }

        [Column("category")]
        [StringLength(50)]
        public string Category { get; set; }
    }
}

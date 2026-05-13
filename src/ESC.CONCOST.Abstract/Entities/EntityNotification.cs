using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

[Table("SETTING_NOTIFICATION")]
public class EntityNotification : EntityBase
{
    public string TitleEn { get; set; }

    [Display(Name = "Tiêu đề (VI)|제목 (VI)")]
    public string TitleVi { get; set; }

    [Display(Name = "Liên kết|링크")]
    public string Href { get; set; }

    public Guid Guid_Notification { get; set; }

    public Guid Guid_User { get; set; }

    public Guid[] Guid_UserNotification { get; set; }

    [Display(Name = "Ảnh đại diện|프로필 사진")]
    public string Avatar { get; set; }
    
    [Display(Name = "Đã xem|읽음")]
    public bool Check { get; set; } = false;
    
    [Display(Name = "Phân hệ|모듈")]
    public string Module { get; set; }
}
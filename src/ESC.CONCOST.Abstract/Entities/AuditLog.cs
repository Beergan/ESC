using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

[Table("AUDIT_LOGS")]
public class AuditLog : EntityBase
{
    [Display(Name = "Tên tài khoản|사용자 이름")]
    [Column("user_name")]
    public string UserName { get; set; }

    public Guid EmployeeGuid { get; set; }

    [Display(Name = "Địa chỉ IP|IP 주소")]
    public string IpAddress { get; set; }

    [Display(Name = "Loại hành động|동작 유형")]
    [Column("action_type")]
    public string ActionType { get; set; }

    [Display(Name = "Tên bảng|테이블명")]
    [Column("table_name")]
    public string TableName { get; set; }

    public int? RecordId { get; set; }
    public Guid? RecordGuid { get; set; }

    [Display(Name = "Nội dung thay đổi|변경 내용")]
    [Column("change_values")]
    public string ChangeValues { get; set; }

    [Display(Name = "Thời gian|시간")]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

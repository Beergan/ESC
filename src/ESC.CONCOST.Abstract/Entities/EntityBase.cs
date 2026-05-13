using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract;

public class EntityBase
{
    [Key]
    [Column (Order = 1)]
    public int Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(Order = 2)]
    public Guid Guid { get; set; } = Guid.NewGuid();

    [Display(Name ="Ngày tạo|생성일")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Display(Name ="Ngày chỉnh sửa|수정일")]
    public DateTime DateModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Người tạo|생성자")]
    public string UserCreated { get; set; } = "System";

    [Display(Name ="Người chỉnh sửa|수정자")]
    public string UserModified { get; set; } = "System";
}
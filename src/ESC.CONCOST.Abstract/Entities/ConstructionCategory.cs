using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract.Entities;

[Table("CONSTRUCTION_CATEGORY")]
public class ConstructionCategory : EntityBase
{
    [Required]
    [MaxLength(200)]
    [Display(Name = "Tên danh mục|카테고리명")]
    public string Name { get; set; } = string.Empty; // KO support

    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty; // EN support

    [MaxLength(500)]
    [Display(Name = "Mô tả|설명")] 
    public string? Description { get; set; } = "";

    [Display(Name = "Thứ tự|정렬 순서")]
    public int SortOrder { get; set; } = 0;

    [Display(Name = "Trạng thái|상태")]
    public bool IsActive { get; set; } = true;
}

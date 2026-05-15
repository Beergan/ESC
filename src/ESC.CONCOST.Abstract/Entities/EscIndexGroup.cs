using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract;

[Table("ESC_INDEX_GROUPS")]
public class EscIndexGroup : EntityBase
{
    [Column("group_code")]
    [Required]
    [StringLength(50)]
    public string GroupCode { get; set; } = string.Empty;

    [Column("group_name")]
    [Required]
    [StringLength(255)]
    public string GroupName { get; set; } = string.Empty;

    [Column("group_name_en")]
    [StringLength(255)]
    public string GroupNameEn { get; set; } = string.Empty;

    [Column("description")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Column("icon_class")]
    [StringLength(100)]
    public string IconClass { get; set; } = "fa-solid fa-layer-group";

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public virtual ICollection<IndexType> IndexTypes { get; set; } = new List<IndexType>();
}
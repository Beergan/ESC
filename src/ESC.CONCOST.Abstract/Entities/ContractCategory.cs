using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract.Entities;

[Table("CONTRACT_CATEGORY")]
public class ContractCategory : EntityBase
{
    [Column("name")]
    [Required]
    [StringLength(200)]
    public string Name { get; set; } // KO|EN support
    [Column("nameEn")]
    [Required]
    [StringLength(200)] 
    public string NameEn { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

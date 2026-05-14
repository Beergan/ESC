using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("INDEX_TYPES")]
    public class IndexType : EntityBase
    {
        [Column("index_name")]
        [Required]
        [StringLength(255)]
        public string IndexName { get; set; } = string.Empty;

        [Column("index_name_en")]
        [StringLength(255)]
        public string IndexNameEn { get; set; } = string.Empty;

        [Column("index_code")]
        [Required]
        [StringLength(100)]
        public string IndexCode { get; set; } = string.Empty;

        [Column("group_code")]
        [Required]
        [StringLength(50)]
        public string GroupCode { get; set; } = BasicCodes.EscIndexGroup.Material;

        [Column("index_type")]
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = BasicCodes.EscIndexValueType.Value;

        [Column("data_source")]
        [StringLength(255)]
        public string DataSource { get; set; } = string.Empty;

        [Column("unit")]
        [StringLength(50)]
        public string Unit { get; set; } = string.Empty;

        [Column("update_freq")]
        [StringLength(50)]
        public string UpdateFreq { get; set; } = "MONTHLY";

        [Column("is_ppi_type")]
        public bool IsPpiType { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<IndexTimeSeries> TimeSeries { get; set; } = new List<IndexTimeSeries>();
    }
}
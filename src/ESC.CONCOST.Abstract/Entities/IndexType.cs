using System;
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
        public string IndexName { get; set; }

        [Column("index_type")]
        [StringLength(50)]
        public string Type { get; set; } // unit_price or index

        [Column("data_source")]
        [StringLength(255)]
        public string DataSource { get; set; }

        [Column("unit")]
        [StringLength(50)]
        public string Unit { get; set; }

        [Column("update_freq")]
        [StringLength(50)]
        public string UpdateFreq { get; set; }

        [Column("is_ppi_type")]
        public bool IsPpiType { get; set; } = false;

        // Relationships
        public virtual ICollection<IndexTimeSeries> TimeSeries { get; set; }
    }
}

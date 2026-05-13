using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("INDEX_TIMESERIES")]
    public class IndexTimeSeries : EntityBase
    {
        [Column("period_key")]
        [Required]
        [StringLength(6)]
        public string PeriodKey { get; set; } // YYYYMM

        [Column("index_key")]
        [Required]
        [StringLength(50)]
        public int? IndexKey { get; set; }

        [Column("index_value")]
        public decimal IndexValue { get; set; }

        [Column("data_verified")]
        public bool DataVerified { get; set; } = false;

        // Navigation Properties
        [ForeignKey("IndexKey")]
        public virtual IndexType IndexType { get; set; }
    }
}

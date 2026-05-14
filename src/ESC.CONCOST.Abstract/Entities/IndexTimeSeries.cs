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
        public string PeriodKey { get; set; } = string.Empty; // YYYYMM

        [Column("year")]
        public int Year { get; set; }

        [Column("month")]
        public int Month { get; set; }

        // Nếu DB cũ đang dùng index_key thì để Column là "index_key".
        // Nếu DB mới thì dùng "index_type_id".
        [Column("index_key")]
        [Required]
        public int IndexTypeId { get; set; }

        [Column("index_value", TypeName = "decimal(18,6)")]
        public decimal IndexValue { get; set; }

        [Column("data_verified")]
        public bool DataVerified { get; set; }

        [ForeignKey(nameof(IndexTypeId))]
        public virtual IndexType? IndexType { get; set; }
    }
}
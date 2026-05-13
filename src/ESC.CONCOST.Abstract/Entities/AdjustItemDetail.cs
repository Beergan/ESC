using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ADJUST_ITEM_DETAILS")]
    public class AdjustItemDetail : EntityBase
    {
        [Column("record_id")]
        [Required]
        public int? RecordId { get; set; }

        [Column("item_id")]
        [Required]
        public int? ItemId { get; set; }

        [Column("index_key")]
        [StringLength(50)]
        public string IndexKey { get; set; }

        [Column("index0")]
        public decimal Index0 { get; set; }

        [Column("index1")]
        public decimal Index1 { get; set; }

        [Column("ki_value")]
        public decimal KiValue { get; set; }

        [Column("weight")]
        public decimal Weight { get; set; }

        [Column("wi_ki")]
        public decimal WiKi { get; set; }

        [Column("amount")]
        public long Amount { get; set; }

        [Column("is_manual")]
        public bool IsManual { get; set; } = false;

        // Navigation Properties
        [ForeignKey("RecordId")]
        public virtual AdjustRecord AdjustRecord { get; set; }

        [ForeignKey("ItemId")]
        public virtual ContractItem ContractItem { get; set; }
    }
}

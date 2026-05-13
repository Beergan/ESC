using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ADJUST_RECORDS")]
    public class AdjustRecord : EntityBase
    {
        [Column("contract_id")]
        [Required]
        public int? ContractId { get; set; }

        [Column("adjust_no")]
        public int AdjustNo { get; set; }

        [Column("bid_date_used")]
        public DateTime BidDateUsed { get; set; }

        [Column("compare_date_used")]
        public DateTime CompareDateUsed { get; set; }

        [Column("elapsed_days")]
        public int ElapsedDays { get; set; }

        [Column("kd_value")]
        public decimal KdValue { get; set; }

        [Column("apply_amount")]
        public long ApplyAmount { get; set; }

        [Column("gross_adjust")]
        public long GrossAdjust { get; set; }

        [Column("advance_deduct")]
        public long AdvanceDeduct { get; set; }

        [Column("net_adjust")]
        public long NetAdjust { get; set; }

        [Column("threshold_met")]
        public bool ThresholdMet { get; set; }

        [Column("days_met")]
        public bool DaysMet { get; set; }

        // Navigation Properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public virtual ICollection<AdjustItemDetail> Details { get; set; }
    }
}

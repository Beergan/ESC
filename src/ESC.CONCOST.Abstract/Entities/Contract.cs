using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("CONTRACTS")]
    public class Contract : EntityBase
    {
        [Column("customer_id")]
        [Required]
        public int? CustomerId { get; set; }

        [Column("project_name")]
        [Required]
        [StringLength(255)]
        public string ProjectName { get; set; }

        [Column("contractor")]
        [StringLength(255)]
        public string Contractor { get; set; }

        [Column("client")]
        [StringLength(255)]
        public string Client { get; set; }

        [Column("contract_method")]
        [StringLength(50)]
        public string ContractMethod { get; set; }

        [Column("bid_rate")]
        public decimal BidRate { get; set; }

        [Column("contract_date")]
        public DateTime ContractDate { get; set; }

        [Column("contract_amount")]
        public long ContractAmount { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("completion_date")]
        public DateTime CompletionDate { get; set; }

        [Column("bid_date")]
        public DateTime BidDate { get; set; }

        [Column("compare_date")]
        public DateTime CompareDate { get; set; }

        [Column("adjust_no")]
        public int AdjustNo { get; set; } = 1;

        [Column("advance_amt")]
        public long AdvanceAmt { get; set; }

        [Column("excluded_amt")]
        public long ExcludedAmt { get; set; }

        [Column("threshold_rate")]
        public decimal ThresholdRate { get; set; } = 0.03m;

        [Column("threshold_days")]
        public int ThresholdDays { get; set; } = 90;

        [Column("work_type")]
        [StringLength(50)]
        public string WorkType { get; set; }

        [Column("prepared_by")]
        [StringLength(255)]
        public string PreparedBy { get; set; }

        [Column("previous_month")]
        [StringLength(7)]
        public string PreviousMonth { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<ContractItem> Items { get; set; }
        public virtual ICollection<AdjustRecord> AdjustRecords { get; set; }
    }
}

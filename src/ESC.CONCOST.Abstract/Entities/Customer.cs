using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("CUSTOMER")]
    public class Customer : EntityBase
    {
        [Column("company_name")]
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [Column("business_license")]
        [StringLength(50)]
        public string BusinessLicense { get; set; }

        [Column("ceo_name")]
        [StringLength(100)]
        public string CeoName { get; set; }

        [Column("approval_status")]
        public int ApprovalStatus { get; set; } = 0; // 0: Pending, 1: Approved, 2: Rejected

        [Column("request_date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Column("reject_reason")]
        public string RejectReason { get; set; } 

        [Column("is_paid")]
        public bool IsPaid { get; set; } = false;

        [Column("membership_type")]
        [StringLength(20)]
        public string MembershipType { get; set; } = "Free";
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}

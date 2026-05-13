using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("ESC_SERVICE_REQUEST")]
    public class EscServiceRequest : EntityBase
    {

        [Column("customer_id")]
        [Required]
        public int? CustomerId { get; set; }

        [Column("contract_id")]
        public int? ContractId { get; set; }

        [Column("request_date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("admin_note")]
        public string AdminNote { get; set; }

        [Column("attachment_path")]
        [StringLength(500)]
        public string AttachmentPath { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }
    }
}

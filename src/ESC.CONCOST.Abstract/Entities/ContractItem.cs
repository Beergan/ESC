using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("CONTRACT_ITEMS")]
    public class ContractItem : EntityBase
    {
        [Column("contract_id")]
        [Required]
        public int? ContractId { get; set; }

        [Column("item_code")]
        [Required]
        [StringLength(20)]
        public string ItemCode { get; set; }

        [Column("group_name")]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Column("item_name")]
        [Required]
        [StringLength(255)]
        public string ItemName { get; set; }

        [Column("index_key")]
        public int? IndexKey { get; set; }

        [Column("amount")]
        public long Amount { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        [ForeignKey("IndexKey")]
        public virtual IndexType IndexType { get; set; }
    }
}

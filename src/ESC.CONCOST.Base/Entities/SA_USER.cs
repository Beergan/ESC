using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract;

public class SA_USER : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Note { get; set; } = string.Empty;
    public Guid? GuidEmployee { get; set; }
    public bool EmployeeConnected { get; set; }
    public string Avatar { get; set; }
    public bool Active { get; set; }

    public string Logo { get; set; }
    public Guid? CompanyGuid { get; set; } = Guid.Empty;

    public int? CustomerId { get; set; }

    [Column("approved_by")]
    [StringLength(36)]
    public string ApprovedBy { get; set; } = string.Empty;  // Default empty, not null

    [Column("approved_date")]
    public DateTime? ApprovedDate { get; set; }
    // Navigation Properties
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}
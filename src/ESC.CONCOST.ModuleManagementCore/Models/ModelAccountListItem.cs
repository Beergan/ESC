using ESC.CONCOST.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleManagementCore.Models;

public class ModelAccountListItem
{
    public string AccountType { get; set; } = BasicCodes.AccountType.Admin;

    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = "-";

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool? Active { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Guid? GuidEmployee { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public Guid? CustomerGuid { get; set; }

    public int? CustomerId { get; set; }

    public string BusinessLicense { get; set; } = string.Empty;

    public string CeoName { get; set; } = string.Empty;

    public int? CustomerApprovalStatus { get; set; }

    public bool? IsPaid { get; set; }

    public string MembershipType { get; set; } = string.Empty;

    public DateTime? RequestDate { get; set; }

    public bool HasUser => !string.IsNullOrWhiteSpace(UserName) && UserName != "-";

    public bool IsEmployee => AccountType == BasicCodes.AccountType.Employee;

    public bool IsCustomer => AccountType == BasicCodes.AccountType.Customer;

    public bool IsAdmin => AccountType == BasicCodes.AccountType.Admin;
}
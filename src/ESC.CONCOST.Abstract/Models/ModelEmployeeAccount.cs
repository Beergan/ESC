using System;

namespace ESC.CONCOST.Abstract;

public class ModelEmployeeAccount
{
    public Guid GuidEmployee { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{LastName} {FirstName}";

    public DateTime? DateOfBirth { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string UserName { get; set; }

    public bool? Locked { get; set; }

    public string Note { get; set; }

    public Guid OfficeGuid { get; set; }

    public Guid JobGuid { get; set; }
    public string? JobNameVi { get; set; }
    public string? JobNameKo { get; set; }

    public bool? EmployeeContected { get; set; }
    public DateTime DateCreated { get; set; }
}
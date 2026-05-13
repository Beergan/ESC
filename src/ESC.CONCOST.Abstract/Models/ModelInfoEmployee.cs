using System;
using System.ComponentModel.DataAnnotations;

namespace ESC.CONCOST.Abstract;

public class ModelInfoEmployee
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public Guid OfficeGuid { get; set; }
    public string? OfficeNameVi { get; set; }
    public string? OfficeNameKo { get; set; }
    public string FirstName { get; set; }
    public string FullName { get; set; }
    public string Gender { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Avatar { get; set; }
    public bool Active { get; set; }
    public Guid JobGuid { get; set; }
    public string? JobNameVi { get; set; }
    public string? JobNameKo { get; set; }
}
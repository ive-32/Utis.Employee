using System.ComponentModel.DataAnnotations;

namespace Employee.Data.Entities;

public class EmployeeEntity
{
    public long Id { get; set; }

    [MaxLength(450)]
    public string FirstName { get; set; } = default!;

    [MaxLength(450)]
    public string LastName { get; set; } = default!;

    [MaxLength(450)]
    public string MiddleName { get; set; } = default!;

    public DateOnly BirthDay { get; set; }

    public int Sex { get; set; }

    public bool HaveChildren { get; set; }
}
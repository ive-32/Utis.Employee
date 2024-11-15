using System.ComponentModel.DataAnnotations;

namespace Employee.Data.Configuration;

public class EmployeeDbContextOptions
{
    [Required(AllowEmptyStrings = false)]
    public string ConnectionString { get; set; } = default!;
}

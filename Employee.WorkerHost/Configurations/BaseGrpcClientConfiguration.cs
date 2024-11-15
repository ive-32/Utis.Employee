using System.ComponentModel.DataAnnotations;

namespace Employee.WorkerHost.Configurations;

public class BaseGrpcClientConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public string Address { get; set; } = default!;
}
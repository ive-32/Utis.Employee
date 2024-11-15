using System.ComponentModel.DataAnnotations;

namespace Employee.WorkerHost.Configurations;

public class WorkerIntegrationConfiguration : BaseGrpcClientConfiguration
{
    [Required]
    public int RequestInterval { get; set; }
}
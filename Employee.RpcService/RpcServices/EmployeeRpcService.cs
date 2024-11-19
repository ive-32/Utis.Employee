using Employee.Proto;
using Employee.RpcService.Helpers;
using Employee.RpcService.Services;
using Grpc.Core;
using Utis.Minex.WrokerIntegration;
using EmployeeService = Employee.Proto.EmployeeService; 
namespace Employee.RpcService.RpcServices;

public class EmployeeRpcService : EmployeeService.EmployeeServiceBase
{
    private readonly ILogger<EmployeeRpcService> _logger;
    private readonly IEmployeeService _employeeService;

    public EmployeeRpcService(ILogger<EmployeeRpcService> logger, IEmployeeService employeeService)
    {
        _logger = logger;
        _employeeService = employeeService;
    }

    public override async Task<IdModel> Create(WorkerMessage request, ServerCallContext context)
    {
        _logger.LogTrace("Create");
        var id = await _employeeService.Create(request, context.CancellationToken);
        return new IdModel()
        {
            Id = id 
        };
    }

    public override async Task<EmptyMessage> Update(WorkerMessage request, ServerCallContext context)
    {
        _logger.LogTrace("Update");
        var id = request.Id.EnsureNotNull();
        await _employeeService.Update(id.Value, request, context.CancellationToken);
        return new EmptyMessage();
    }

    public override async Task<EmptyMessage> Delete(IdModel request, ServerCallContext context)
    {
        _logger.LogTrace("Delete");
        await _employeeService.Delete(request, context.CancellationToken);
        return new EmptyMessage();
    }

    public override async Task<WorkerMessages> GetList(GetListModel request, ServerCallContext context)
    {
        _logger.LogTrace("GetList");
        var result = await _employeeService.GetList(request, context.CancellationToken);
        return new WorkerMessages() { Workers = { result } };
    }
}
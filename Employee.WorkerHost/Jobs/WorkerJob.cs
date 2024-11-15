using Employee.Proto;
using Employee.WorkerHost.Clients;
using Grpc.Core;
using Quartz;

namespace Employee.WorkerHost.Jobs;

[DisallowConcurrentExecution]
public class WorkerJob :IJob
{
    private readonly WorkerIntegrationClient _workerClient;
    private readonly EmployeeServiceClient _employeeServiceClient;
    private readonly ILogger<WorkerJob> _logger;

    public WorkerJob(WorkerIntegrationClient workerClient, EmployeeServiceClient employeeServiceClient, 
        ILogger<WorkerJob> logger)
    {
        _workerClient = workerClient;
        _employeeServiceClient = employeeServiceClient;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace("Execute GetWorkerStreamAsync");
        try
        {
            var actionCounter = 0;
            await foreach (var action in _workerClient.GetWorkerStreamAsync())
            {
                try
                {
                    switch (action.ActionType)
                    {
                        case Utis.Minex.WrokerIntegration.Action.Create:
                            _ = await _employeeServiceClient.CreateAsync(action.Worker);
                            break;
                        case Utis.Minex.WrokerIntegration.Action.Update:
                            _ = await _employeeServiceClient.UpdateAsync(action.Worker);
                            break;
                        case Utis.Minex.WrokerIntegration.Action.Delete:
                            if (action.Worker.Id is not null)
                                _ = await _employeeServiceClient.DeleteAsync(new IdModel()
                                    { Id = action.Worker.Id.Value });
                            break;
                    }

                    actionCounter++;
                }
                catch (RpcException e)
                {
                    _logger.LogError("{actionType} throw error {errorMessage} in Employee Service", action.ActionType,
                        e.Message);
                }
            }
            _logger.LogInformation("{count} actions proceed", actionCounter);
        }
        catch (RpcException e)
        {
            _logger.LogError("Service WorkerIntegration throw error {errorMessage}", e.Message);
        }
    }
}
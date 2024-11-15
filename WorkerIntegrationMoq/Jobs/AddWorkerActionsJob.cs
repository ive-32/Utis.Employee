using Employee.Proto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Quartz;
using Utis.Minex.WrokerIntegration;
using WorkerIntegrationMoq.Configurations;
using Action = Utis.Minex.WrokerIntegration.Action;

namespace WorkerIntegrationMoq.Jobs;

[DisallowConcurrentExecution]
public class AddWorkerActionsJob :IJob
{
    private static readonly Random Random = new Random();
    private readonly EmployeeService.EmployeeServiceClient _employeeServiceClient;
    private readonly ILogger<AddWorkerActionsJob> _logger;
    private readonly IOptions<AddWorkerActionsJobConfiguration> _options;

    public AddWorkerActionsJob(EmployeeService.EmployeeServiceClient employeeServiceClient, 
        ILogger<AddWorkerActionsJob> logger, IOptions<AddWorkerActionsJobConfiguration> options)
    {
        _employeeServiceClient = employeeServiceClient;
        _logger = logger;
        _options = options;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (ActionsQueue.Actions.Count > _options.Value.MaxQueueSize)
            return;
        
        WorkerMessages? existingEmployers = null;
        try
        {
            existingEmployers = await _employeeServiceClient.GetListAsync(new GetListModel());
        }
        catch (RpcException ex)
        {
            _logger.LogError("Error while get employee list {}", ex.Message);
        }
        
        var count = Random.Next(_options.Value.MinActionsPerJob, _options.Value.MaxActionsPerJob + 1);
        _logger.LogInformation("Add {count} workerActions", count);
        
        for (var i = 0; i < count; i++)
        {
            var existingWorkerId = new Int64Value();
            var actionType = Action.Create;
            if (existingEmployers is not null && existingEmployers.Workers.Count > i)
            {
                existingWorkerId = existingEmployers.Workers[i].Id;
                actionType = (Action) Random.Next(1, 4);
            }

            var workerMessage = new WorkerMessage
            {
                Id = actionType switch
                {
                    Action.Update => existingWorkerId,
                    Action.Delete => existingWorkerId,
                    _ => null
                },
                LastName = actionType switch
                {
                    Action.Update => "Updated",
                    _ => Guid.NewGuid().ToString()
                },
                FirstName = Guid.NewGuid().ToString(),
                MiddleName = Guid.NewGuid().ToString(),
                Birthday = GetRandomBirthday(),
                Sex = (Sex)Random.Next(1, 3),
                HaveChildren = Random.Next(0, 2) == 1,
            };

            var workerAction = new WorkerAction
            {
                Worker = workerMessage,
                ActionType = actionType
            };
            
            workerMessage.Id = workerAction.ActionType switch
            {
                Action.Update => existingWorkerId,
                Action.Delete => existingWorkerId,
                _ => workerMessage.Id
            };
            
            ActionsQueue.Actions.Enqueue(workerAction);
        }
    }
        
    private long GetRandomBirthday()
        => (long)(DateTime.UtcNow.AddDays(- Random.Next(365 * 22, 365 * 65)) - new DateTime(1970, 1, 1)).TotalSeconds;

}
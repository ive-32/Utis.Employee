using Grpc.Core;
using Microsoft.Extensions.Options;
using Utis.Minex.WrokerIntegration;
using WorkerIntegrationMoq.Configurations;

namespace WorkerIntegrationMoq.Services;

public class WorkerIntegrationMoq : WorkerIntegration.WorkerIntegrationBase
{
    private readonly ILogger<WorkerIntegrationMoq> _logger;
    private readonly IOptions<WorkerIntegrationConfiguration> _options;

    public WorkerIntegrationMoq(ILogger<WorkerIntegrationMoq> logger, 
        IOptions<WorkerIntegrationConfiguration> options)
    {
        _logger = logger;
        _options = options;
    }
    
    public override async Task GetWorkerStream(EmptyMessage request, 
        IServerStreamWriter<WorkerAction> responseStream,
        ServerCallContext context)
    {
        _logger.LogTrace("GetWorkerStream");
        
        for (var i = 0; i < _options.Value.MaxStreamSize; i++)
        {       
            if (ActionsQueue.Actions.TryDequeue(out var workerAction))
                    await responseStream.WriteAsync(workerAction);
        }
    }
}
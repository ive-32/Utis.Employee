using Employee.WorkerHost.Configurations;
using Grpc.Core;
using Grpc.Net.Client;
using Utis.Minex.WrokerIntegration;

namespace Employee.WorkerHost.Clients;

public class WorkerIntegrationClient
{
    private readonly WorkerIntegration.WorkerIntegrationClient _client;
    private readonly ILogger<WorkerIntegrationClient> _logger; 
    public WorkerIntegrationClient(WorkerIntegrationConfiguration options, ILogger<WorkerIntegrationClient> logger)
    {
        var channel = GrpcChannel.ForAddress(options.Address);
        _client = new WorkerIntegration.WorkerIntegrationClient(channel);
        _logger = logger;
    }

    public async IAsyncEnumerable<WorkerAction> GetWorkerStreamAsync()
    {
        _logger.LogTrace("GetWorkerStream");
        using var call = _client.GetWorkerStream(new EmptyMessage());
        while (await call.ResponseStream.MoveNext())
        {
            yield return call.ResponseStream.Current;
        }
    }
}

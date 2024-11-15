using Employee.Proto;
using Employee.WorkerHost.Configurations;
using Grpc.Net.Client;
using Utis.Minex.WrokerIntegration;

namespace Employee.WorkerHost.Clients;

public class EmployeeServiceClient
{
    private readonly EmployeeService.EmployeeServiceClient _client;

    public EmployeeServiceClient(EmployeeClientConfiguration options)
    {
        var channel = GrpcChannel.ForAddress(options.Address);
        _client = new EmployeeService.EmployeeServiceClient(channel);
    }

    public async Task<IdModel> CreateAsync(WorkerMessage message)
    {
        return await _client.CreateAsync(message);
    }

    public async Task<EmptyMessage> UpdateAsync(WorkerMessage message)
    {
        return await _client.UpdateAsync(message);
    }
    
    public async Task<EmptyMessage> DeleteAsync(IdModel message)
    {
        return await _client.DeleteAsync(message);
    }
}

using Employee.Proto;
using Utis.Minex.WrokerIntegration;

namespace Employee.Service.Services;

public interface IEmployeeService
{
    Task<long> Create(WorkerMessage request, CancellationToken ct);
    Task Update(long id, WorkerMessage request, CancellationToken ct);
    Task Delete(IdModel request, CancellationToken ct);
    Task<IReadOnlyList<WorkerMessage>> GetList(GetListModel request, CancellationToken ct);
}
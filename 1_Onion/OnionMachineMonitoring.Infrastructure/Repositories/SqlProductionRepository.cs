using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Domain.Interfaces;

namespace MachineMonitoring.Infrastructure;

/// <summary>Source: SQL (stub).</summary>
public sealed class SqlProductionRepository : IProductionRepository
{
    public Task<IReadOnlyList<MachineProduction>> FetchAsync(object range)
        => Task.FromResult<IReadOnlyList<MachineProduction>>(new List<MachineProduction>());

    public Task<IReadOnlyList<MachineProduction>> GetByMachineAndRangeAsync(int machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpsertAsync(IReadOnlyList<MachineProduction> items)
        => Task.CompletedTask; // SQL source is not the sink in our setup

    public Task UpsertRangeAsync(IEnumerable<MachineProduction> items)
    {
        throw new NotImplementedException();
    }
}

using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Domain.Interfaces;

namespace MachineMonitoring.Infrastructure;

/// <summary>Source: CSV (stub; parsing/mapping only).</summary>
public sealed class CsvProductionRepository : IProductionRepository
{
    public Task<IReadOnlyList<MachineProduction>> FetchAsync(
        object range, 
        CancellationToken ct)
        => Task.FromResult<IReadOnlyList<MachineProduction>>(new List<MachineProduction>());

    public Task<IReadOnlyList<MachineProduction>> FetchAsync(object range)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<MachineProduction>> GetByMachineAndRangeAsync(int machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpsertAsync(
        IReadOnlyList<MachineProduction> items, 
        CancellationToken ct)
        => Task.CompletedTask;

    public Task UpsertAsync(IReadOnlyList<MachineProduction> items)
    {
        throw new NotImplementedException();
    }

    public Task UpsertRangeAsync(IEnumerable<MachineProduction> items)
    {
        throw new NotImplementedException();
    }
}

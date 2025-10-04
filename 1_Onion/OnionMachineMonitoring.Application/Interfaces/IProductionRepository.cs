using OnionMachineMonitoring.Core.Entities;

namespace OnionMachineMonitoring.Domain.Interfaces
{
    public interface IProductionRepository
    {
        Task<IReadOnlyList<MachineProduction>> GetByMachineAndRangeAsync(
            int machineId, 
            DateTime fromUtc, 
            DateTime toUtc, 
            CancellationToken ct);
        
        Task UpsertRangeAsync(IEnumerable<MachineProduction> items);

        Task<IReadOnlyList<MachineProduction>> FetchAsync(object range);

        Task UpsertAsync(IReadOnlyList<MachineProduction> items);
    }
}

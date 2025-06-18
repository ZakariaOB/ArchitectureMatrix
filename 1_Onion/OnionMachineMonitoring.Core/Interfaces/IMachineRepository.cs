using OnionMachineMonitoring.Core.Entities;

namespace OnionMachineMonitoring.Core.Interfaces;

public interface IMachineRepository
{
    Task<IEnumerable<Machine>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Machine> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Machine machine, CancellationToken cancellationToken = default);
    Task UpdateAsync(Machine machine, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

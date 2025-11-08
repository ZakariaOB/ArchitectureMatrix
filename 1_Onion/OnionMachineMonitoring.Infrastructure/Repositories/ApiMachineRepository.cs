using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;

namespace ArchitectureMatrix.DependencyInversion.Onion_IEnumerableFix.Infrastructure;

// ✅ API or file-based implementation
public class ApiMachineRepository : IMachineRepository
{
    private readonly IEnumerable<Machine> _machines;

    public ApiMachineRepository(IEnumerable<Machine> machines)
    {
        _machines = machines;
    }

    public Task AddAsync(Machine machine, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Machine>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Machine> GetAllMachines() => _machines;

    public Task<Machine> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Machine machine, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    IEnumerable<Machine> IMachineRepository.GetAllMachines()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Machine> GetMachinesByPrefix(char prefix)
        => _machines.Where(m => m.Name.StartsWith(prefix));
}

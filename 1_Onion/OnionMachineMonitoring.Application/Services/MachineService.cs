using OnionMachineMonitoring.Application.Interfaces;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;

namespace OnionMachineMonitoring.Application.Services;

public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;

    public MachineService(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public Task<IEnumerable<Machine>> GetAllAsync(CancellationToken cancellationToken = default)
        => _machineRepository.GetAllAsync(cancellationToken);

    public Task<Machine> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _machineRepository.GetByIdAsync(id, cancellationToken);

    public Task AddAsync(Machine machine, CancellationToken cancellationToken = default)
        => _machineRepository.AddAsync(machine, cancellationToken);

    public async Task AddAsync(int id, string name, string? description = null, CancellationToken cancellationToken = default)
    {
        var machine = new Machine(id, name, description);
        await _machineRepository.AddAsync(machine, cancellationToken);
    }


    public Task UpdateAsync(Machine machine, CancellationToken cancellationToken = default)
        => _machineRepository.UpdateAsync(machine, cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _machineRepository.DeleteAsync(id, cancellationToken);
}

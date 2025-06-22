using MachineMonitoring.Shared.Enums;
using OnionMachineMonitoring.Application.Interfaces;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;
using System.Net.Http.Json;

namespace OnionMachineMonitoring.Application.Services;

public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly HttpClient _httpClient;

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

    public async Task<EntityDeleteResult> DeleteIfAllowedAsync(int id)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
            return EntityDeleteResult.NotFound;

        try
        {
            machine.EnsureCanBeDeleted(DateTime.UtcNow); // Domain rule enforced
        }
        catch (Exception)
        {
            return EntityDeleteResult.Forbidden;
        }

        await _machineRepository.DeleteAsync(machine.Id); // commit handled internally
        return EntityDeleteResult.Deleted;
    }

    public async Task<IEnumerable<Machine>> GetMachinesAsync(string source = "db")
    {
        if (source == "db")
        {
            return await _machineRepository.GetAllAsync(default); // tightly coupled
        }
        else if (source == "http")
        {
            var response = await _httpClient.GetFromJsonAsync<List<Machine>>("https://external/api/machines");
            return response;
        }

        throw new NotSupportedException("Unknown source");
    }
}

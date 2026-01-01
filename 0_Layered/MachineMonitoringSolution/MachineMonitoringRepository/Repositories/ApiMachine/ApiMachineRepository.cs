using MachineMonitoring.Shared.Enums;
using MachineMonitoringRepository.Models;

namespace MachineMonitoring.Repository.Repositories.ApiMachine;

/// <summary>
/// Simulates a non-EF repository (e.g., REST API client or file-based data source)
/// 
/// DEMONSTRATES: What happens when you try to swap EF repository with another implementation
/// This repository CANNOT provide IQueryable - API calls return materialized data
/// </summary>
public class ApiMachineRepository : IMachineRepository
{
    private readonly List<Machine> machines =
    [
        new Machine { Name = "Cutter" },
        new Machine { Name = "Printer" },
        new Machine { Name = "Folder" }
    ];

    public Task<EntityDeleteResult> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Machine>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Machine> GetAllMachines()
    {
        return machines;
    }

    public Task<Machine> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}

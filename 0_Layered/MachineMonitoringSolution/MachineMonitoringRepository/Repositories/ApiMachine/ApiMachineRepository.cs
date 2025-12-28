using MachineMonitoring.Shared.Enums;
using MachineMonitoringRepository.Models;
using MachineMonitoringRepository.Repositories;
using Microsoft.EntityFrameworkCore; // to simulate EF dependency difference
using System.Linq;

namespace ArchitectureMatrix.DependencyInversion.Layered_IQueryableLeak;

// ❌ Simulates a non-EF repository (e.g., REST API or CSV)
public class ApiMachineRepository : IMachineRepository
{
    private readonly IEnumerable<Machine> _machines =
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
        // Works syntactically, but EF-specific functions will fail at runtime
        return _machines.AsQueryable();
    }

    public Task<Machine> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}

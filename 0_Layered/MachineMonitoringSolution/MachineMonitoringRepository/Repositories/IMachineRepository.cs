using MachineMonitoring.Shared.Enums;
using MachineMonitoringRepository.Models;
using MachineMonitoringRepository.Repositories.BaseRepository;

namespace MachineMonitoring.Repository.Repositories
{
    /// <summary>
    /// LAYERED ARCHITECTURE PROBLEM:
    /// 
    /// This interface returns IQueryable<Machine> - explicitly leaking infrastructure
    /// 
    /// WHY THIS IS A PROBLEM:
    /// - Service layer can use EF-specific features (EF.Functions)
    /// - Creates tight coupling to Entity Framework
    /// - Cannot swap implementations (API repository can't provide IQueryable)
    /// 
    /// This demonstrates why explicit IQueryable in interfaces is bad
    /// </summary>
    public interface IMachineRepository : IRepository<Machine>
    {
        IEnumerable<Machine> GetAllMachines();
    }
}

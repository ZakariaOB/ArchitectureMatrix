using MachineMonitoring.Repository.DataContext;
using MachineMonitoring.Shared.Enums;
using MachineMonitoringRepository.Models;
using MachineMonitoringRepository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MachineMonitoring.Repository.Repositories;

public class MachineRepository(MachineMonitoringContext dbContext) : IMachineRepository
{
    private readonly MachineMonitoringContext _dbContext = dbContext;

    private readonly List<Machine> _machines =
    [
        new() { Name = "Cutter" },
        new() { Name = "Printer" },
        new() { Name = "Folder" }
    ];

    public async Task<Machine> GetByIdAsync(int id)
    {
        return await _dbContext.Set<Machine>()
                .Include(machine => machine.MachineProductions)
                .AsNoTracking()
                .FirstOrDefaultAsync(el => el.MachineId == id);
    }

    public async Task<IEnumerable<Machine>> GetAllAsync()
    {
        return await _dbContext.Set<Machine>()
            .Include(machine => machine.MachineProductions)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// The EntityDeleteResult is used because entity framewok will return also related entities count
    /// after deleting a machine (Machine productions count also) . This will help to return 0 or 1 .
    /// </summary>
    /// <param name="machineId">Machine Id</param>
    /// <returns>Entity delete result</returns>
    public async Task<EntityDeleteResult> Delete(int machineId)
    {
        Machine machineToDelete = await GetByIdAsync(machineId);
        if (machineToDelete == null)
        {
            return EntityDeleteResult.NoDeletion;
        }
        _dbContext.Set<Machine>().Remove(machineToDelete);
        return _dbContext.SaveChanges() > 0 ? EntityDeleteResult.Deleted: EntityDeleteResult.NoDeletion;
    }

    public IQueryable<Machine> GetAllMachines() => _dbContext.Machines;
}

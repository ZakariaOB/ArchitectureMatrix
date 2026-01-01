using MachineMonitoring.Repository.DataContext;
using Microsoft.EntityFrameworkCore;

namespace MachineMonitoring.Repository.Repositories;

public class MaintenanceRepository(MachineMonitoringContext db) : IMaintenanceRepository
{
    private readonly MachineMonitoringContext _db = db;

    public async Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync(DateTime cutoff)
    => await _db.MaintenanceTasks
                .Where(t => !t.IsClosed && t.CreatedAt < cutoff)
                .ToListAsync();

    public async Task SaveAsync(MaintenanceTask task)
    {
        var tracked = await _db.MaintenanceTasks.FindAsync(task.Id);
        if (tracked is null)
        {
            _db.MaintenanceTasks.Add(task);
        }
        else
        {
            _db.Entry(tracked).CurrentValues.SetValues(task);
        }

        await _db.SaveChangesAsync();
    }
}

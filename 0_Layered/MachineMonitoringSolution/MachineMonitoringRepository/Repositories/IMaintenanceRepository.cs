namespace MachineMonitoring.Repository.Repositories;

public interface IMaintenanceRepository
{
    Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync(DateTime cutoff);

    Task SaveAsync(MaintenanceTask task);
}

using OnionMachineMonitoring.Domain.Entities;

public class FakeMaintenanceRepository : IMaintenanceRepository
{
    public List<MaintenanceTask> Tasks { get; } = [];

    public Task<IEnumerable<MaintenanceTask>> GetOverdueTasksAsync(DateTime cutoff)
        => Task.FromResult(Tasks.Where(t => !t.IsClosed && t.CreatedAt < cutoff));

    public Task SaveAsync(MaintenanceTask task)
    {
        if (!Tasks.Contains(task)) Tasks.Add(task);
        return Task.CompletedTask;
    }
}

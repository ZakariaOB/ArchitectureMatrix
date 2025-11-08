using OnionMachineMonitoring.Domain.Entities;

namespace OnionMachineMonitoring.Application.Services;

public class RescheduleMaintenanceJob
{
    private readonly IMaintenanceRepository _repo;
    public RescheduleMaintenanceJob(IMaintenanceRepository repo) => _repo = repo;

    public async Task ExecuteAsync(DateTime currentDate)
    {
        var overdue = await _repo.GetOverdueTasksAsync(currentDate.AddDays(-30));
        foreach (var task in overdue)
        {
            task.IsClosed = true;
            await _repo.SaveAsync(task);

            await _repo.SaveAsync(new MaintenanceTask
            {
                MachineId = task.MachineId,
                Failure = task.Failure,
                IsClosed = false,
                RescheduledFrom = task.Id,
                CreatedAt = currentDate
            });
        }
    }
}

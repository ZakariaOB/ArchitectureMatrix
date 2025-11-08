using OnionMachineMonitoring.Application.Services;
using OnionMachineMonitoring.Domain.Entities;

namespace OnionMachineMonitoring.Tests;

public class OnionShouldCloseAndRescheduleTasks
{
    [Fact]
    public async Task Onion_Should_Close_And_Reschedule_Tasks()
    {
        var repo = new FakeMaintenanceRepository();
        var old = new MaintenanceTask
        {
            MachineId = Guid.NewGuid(),
            Failure = "Leak",
            CreatedAt = DateTime.UtcNow.AddDays(-40),
            IsClosed = false
        };
        repo.Tasks.Add(old);

        var job = new RescheduleMaintenanceJob(repo);
        await job.ExecuteAsync(DateTime.UtcNow);

        Assert.True(old.IsClosed);
        Assert.Equal(2, repo.Tasks.Count);

        var rescheduled = repo.Tasks.Last();
        Assert.False(rescheduled.IsClosed);
        Assert.Equal(old.Id, rescheduled.RescheduledFrom);
    }
}

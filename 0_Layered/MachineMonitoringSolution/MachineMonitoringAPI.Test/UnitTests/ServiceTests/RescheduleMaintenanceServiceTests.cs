using MachineMonitoring.Repository.DataContext;
using MachineMonitoring.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MachineMonitoring.Tests.UnitTests.RepoTests;

public class RescheduleMaintenanceServiceTests
{
    [Fact]
    public async Task Layered_Should_Close_And_Reschedule_Tasks()
    {
        var options = new DbContextOptionsBuilder<MachineMonitoringContext>()
            .UseInMemoryDatabase("LayeredTestDb").Options;
        var db = new MachineMonitoringContext(options);

        db.MaintenanceTasks.Add(new MaintenanceTask
        {
            MachineId = Guid.NewGuid(),
            Failure = "Leak",
            CreatedAt = DateTime.UtcNow.AddDays(-40)
        });
        db.SaveChanges();

        var repo = new MaintenanceRepository(db);
        var service = new RescheduleMaintenanceService(repo);

        await service.ExecuteAsync(DateTime.UtcNow);

        Assert.Equal(2, db.MaintenanceTasks.Count());
    }

}

using MachineMonitoring.Repository.DataContext;
using MachineMonitoring.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace MachineMonitoring.Tests.UnitTests.ServiceTests;

public class RescheduleMaintenanceServiceTests
{
    [Fact]
    public async Task Layered_Should_Close_And_Reschedule_Tasks()
    {
        // Arrange - Integration test with EF
        var options = new DbContextOptionsBuilder<MachineMonitoringContext>()
            .UseInMemoryDatabase("LayeredTestDb").Options;
        var db = new MachineMonitoringContext(options);

        db.MaintenanceTasks.Add(new MaintenanceTask
        {
            MachineId = Guid.NewGuid(),
            Failure = "Leak",
            CreatedAt = DateTime.UtcNow.AddDays(-40)
        });
        await db.SaveChangesAsync();

        var repo = new MaintenanceRepository(db);
        var service = new RescheduleMaintenanceService(repo);

        // Act
        await service.ExecuteAsync(DateTime.UtcNow);

        // Assert
        Assert.Equal(2, db.MaintenanceTasks.Count());
    }

    [Fact]
    public async Task Layered_NoEF_Test_Can_Pass_While_Overdue_Selection_Is_Broken()
    {
        // Arrange
        var repo = new Mock<IMaintenanceRepository>();

        // This task is NOT overdue for a 30-day rule
        var recent = new MaintenanceTask
        {
            MachineId = Guid.NewGuid(),
            Failure = "Leak",
            CreatedAt = DateTime.UtcNow.AddDays(-5), // should NOT be selected by repo in real life
            IsClosed = false
        };

        // ❌ We "force" the repository to return it as overdue.
        // This bypasses the real overdue selection rule entirely.
        repo.Setup(r => r.GetOverdueTasksAsync(It.IsAny<DateTime>()))
            .ReturnsAsync([recent]);

        var service = new RescheduleMaintenanceService(repo.Object);

        // Act
        await service.ExecuteAsync(DateTime.UtcNow);

        // Assert
        // ✅ Test passes and looks fine...
        repo.Verify(r => r.SaveAsync(It.IsAny<MaintenanceTask>()), Times.Exactly(2));
        Assert.True(recent.IsClosed);

        // ...but it proved only: "given overdue tasks, service saves twice".
        // It did NOT prove: "repo selects the right overdue tasks".
    }
}

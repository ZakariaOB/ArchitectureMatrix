using ArchitectureMatrix.DependencyInversion.Layered_IQueryableLeak;
using MachineMonitoring.Repository.DataContext;
using MachineMonitoringRepository.Models;
using MachineMonitoringRepository.Repositories;
using MachineMonitoringService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MachineMonitoring.Tests.UnitTests.RepoTests;

public class MachineServiceTests
{
    private static MachineMonitoringContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MachineMonitoringContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new MachineMonitoringContext(options);
        context.Machines.AddRange(
            new Machine { Name = "Cutter" },
            new Machine { Name = "Printer" },
            new Machine { Name = "Folder" }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public void EfRepository_Should_Work_With_IQueryable()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repo = new MachineRepository(context);
        var service = new MachineService(repo, null, null);

        // Act
        var result = service.GetMachineNamesStartingWith('C');

        // Assert
        Assert.Contains("Cutter", result);
    }

    [Fact]
    public void ApiRepository_Should_Fail_Because_It_Cannot_Handle_EF_Functions()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var machines = context.Machines.ToList();  // reuse same data
        var repo = new ApiMachineRepository();
        var service = new MachineService(repo, null, null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = service.GetMachineNamesStartingWith('C').ToList();
        });
    }
}

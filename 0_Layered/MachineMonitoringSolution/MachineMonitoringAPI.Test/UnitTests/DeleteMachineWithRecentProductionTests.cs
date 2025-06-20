using AutoMapper;
using MachineMonitoring.Repository.DataContext;
using MachineMonitoringRepository.Models;
using MachineMonitoringService.Services;
using MachineMonitoringWebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineMonitoring.WebAPI.Test.UnitTests;

public class DeleteMachineWithRecentProductionTests
{
    [Fact]
    public async Task DeleteMachine_WithRecentProduction_ShouldBeForbidden()
    {
        // Arrange
        var dbContext = GetRealDbContext(); // ❌ EF setup required

        var mockService = new Mock<IMachineService>();
        var mockMapper = new Mock<IMapper>();

        var machine = new Machine { Name = "Test" };
        dbContext.Machines.Add(machine);

        dbContext.MachineProductions.Add(new MachineProduction
        {
            MachineId = machine.MachineId,
            TotalProduction = 100,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        });

        await dbContext.SaveChangesAsync();

        var controller = new MachineApiController(
            mockService.Object,
            mockMapper.Object,
            dbContext);

        // Act
        var result = await controller.DeleteMachineWithRecentProduction(machine.MachineId);

        // Assert
        Assert.IsType<ForbidResult>(result); // ✅ rule works
    }

    private static MachineMonitoringContext GetRealDbContext()
    {
        // This uses a fresh in-memory database every time(no side effects between tests).
        var options = new DbContextOptionsBuilder<MachineMonitoringContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensure isolation per test
            .Options;

        return new MachineMonitoringContext(options);
    }
}

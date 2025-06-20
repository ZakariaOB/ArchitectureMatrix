using MachineMonitoring.Shared.Enums;
using Moq;
using OnionMachineMonitoring.Application.Services;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;

namespace OnionMachineMonitoring.Tests
{
    public class MachineServiceTests
    {
        [Fact]
        public async Task DeleteIfAllowedAsync_ShouldReturnForbidden_WhenRecentProductionExists()
        {
            // Arrange
            var machine = new Machine(1, "Test");
            machine.AddProduction(100, DateTime.UtcNow.AddDays(-5));

            var repoMock = new Mock<IMachineRepository>();
            repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(machine);

            var service = new MachineService(repoMock.Object);

            // Act
            var result = await service.DeleteIfAllowedAsync(1);

            // Assert
            Assert.Equal(EntityDeleteResult.Forbidden, result);
        }

        [Fact]
        public async Task DeleteIfAllowedAsync_ShouldReturnDeleted_WhenNoRecentProduction()
        {
            // Arrange
            var machine = new Machine(1, "Test");
            machine.AddProduction(100, DateTime.UtcNow.AddDays(-40));

            var repoMock = new Mock<IMachineRepository>();
            repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(machine);
            repoMock.Setup(r => r.DeleteAsync(machine.Id, default)).Returns(Task.CompletedTask);

            var service = new MachineService(repoMock.Object);

            // Act
            var result = await service.DeleteIfAllowedAsync(1);

            // Assert
            Assert.Equal(EntityDeleteResult.Deleted, result);
            repoMock.Verify(r => r.DeleteAsync(machine.Id, default), Times.Once);
        }

        [Fact]
        public async Task DeleteIfAllowedAsync_ShouldReturnNotFound_WhenMachineDoesNotExist()
        {
            var repoMock = new Mock<IMachineRepository>();
            repoMock.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Machine?)null);

            var service = new MachineService(repoMock.Object);

            var result = await service.DeleteIfAllowedAsync(999);

            Assert.Equal(EntityDeleteResult.NotFound, result);
        }
    }
}

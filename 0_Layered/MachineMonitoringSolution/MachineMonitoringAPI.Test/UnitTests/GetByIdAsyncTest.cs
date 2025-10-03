using MachineMonitoringService.Dto;
using MachineMonitoringWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MachineMonitoring.WebAPI.Test.UnitTests
{
    public class GetByIdAsyncTest: UnitTestBase
    {
        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsOkResult()
        {
            // Arrange
            int validId = 1;
            MachineDto machineDto = new () { MachineId = validId };
            _machineServiceMock.Setup(service => service.GetByIdAsync(validId))
                               .ReturnsAsync(machineDto);

            // Act
            IActionResult result = await _controller.GetByIdAsync(validId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsType<MachineModel>(okResult.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = 100;
            _machineServiceMock.Setup(service => service.GetByIdAsync(invalidId))
                               .ReturnsAsync((MachineDto)null);

            // Act
            IActionResult result = await _controller.GetByIdAsync(invalidId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

using MachineMonitoringService.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MachineMonitoring.WebAPI.Test.UnitTests
{
    public class GetAllAsyncTest : UnitTestBase
    {
        [Fact]
        public async Task GetAllAsync_WithNoMachines_ReturnsNoContent()
        {
            // Arrange
            IEnumerable<MachineDto> emptyMachineList = Enumerable.Empty<MachineDto>();
            _machineServiceMock.Setup(service => service.GetAllAsync())
                               .ReturnsAsync(emptyMachineList);

            // Act
            IActionResult result = await _controller.GetAllAsync();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task GetAllAsync_WithMachines_ReturnsOK()
        {
            // Arrange
            IEnumerable<MachineDto> machineList = new[]
            {
                new MachineDto
                {
                    MachineId = 1
                }
            };
            _machineServiceMock.Setup(service => service.GetAllAsync())
                               .ReturnsAsync(machineList);

            // Act
            IActionResult result = await _controller.GetAllAsync();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}

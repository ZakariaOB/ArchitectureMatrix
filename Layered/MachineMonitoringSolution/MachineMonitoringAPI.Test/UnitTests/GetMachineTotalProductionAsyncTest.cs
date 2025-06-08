using MachineMonitoring.WebAPI.Models;
using MachineMonitoringService.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MachineMonitoring.WebAPI.Test.UnitTests
{
    public class GetMachineTotalProductionAsyncTest : UnitTestBase
    {
        [Fact]
        public async Task GetMachineTotalProductionAsync_WithValidId_ReturnsOkResult()
        {
            // Arrange
            int validId = 1;
            MachineDto machineDto = new() { MachineId = validId };
            _machineServiceMock.Setup(service => service.GetByIdAsync(validId))
                               .ReturnsAsync(machineDto);

            // Act
            IActionResult result = await _controller.GetMachineTotalProductionAsync(validId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.IsType<MachineTotalProductionResult>(okResult.Value);
        }

        [Fact]
        public async Task GetMachineTotalProductionAsync_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = 100;
            _machineServiceMock.Setup(service => service.GetByIdAsync(invalidId))
                               .ReturnsAsync((MachineDto)null);

            // Act
            IActionResult result = await _controller.GetMachineTotalProductionAsync(invalidId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMachineTotalProductionAsync_WithSumVerification()
        {
            // Arrange
            int validId = 1;
            MachineDto machineDto = new() { MachineId = validId , 
                MachineProductions = new List<MachineProductionDto> 
                {
                    new MachineProductionDto
                    {
                        TotalProduction = 50
                    },
                    new MachineProductionDto
                    {
                        TotalProduction = 50
                    },
                    new MachineProductionDto
                    {
                        TotalProduction = 50
                    }
                }
            };
            _machineServiceMock.Setup(service => service.GetByIdAsync(validId))
                               .ReturnsAsync(machineDto);

            // Act
            IActionResult result = await _controller.GetMachineTotalProductionAsync(validId);
            var okResult = result as OkObjectResult;

            // Assert
            Assert.Equal(150, (okResult.Value as MachineTotalProductionResult)?.TotalProduction);
        }
    }
}

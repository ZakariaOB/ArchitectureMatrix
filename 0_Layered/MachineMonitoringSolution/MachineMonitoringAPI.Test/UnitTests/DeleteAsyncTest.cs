using MachineMonitoring.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MachineMonitoring.WebAPI.Test.UnitTests
{
    public class DeleteAsyncTest : UnitTestBase
    {
        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsOkResultWithDeletion()
        {
            // Arrange
            int validId = 1;
            _machineServiceMock.Setup(service => service.Delete(validId))
                               .ReturnsAsync(EntityDeleteResult.Deleted);

            // Act
            IActionResult result = await _controller.DeleteAsync(validId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;    
            Assert.NotNull(okResult);
            Assert.Equal((int)EntityDeleteResult.Deleted, okResult.Value);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsNotDeletion()
        {
            // Arrange
            int invalidId = 1005;
            _machineServiceMock.Setup(service => service.Delete(invalidId))
                               .ReturnsAsync(EntityDeleteResult.NoDeletion);

            // Act
            IActionResult result = await _controller.DeleteAsync(invalidId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal((int)EntityDeleteResult.NoDeletion, notFoundResult.Value);
        }
    }
}

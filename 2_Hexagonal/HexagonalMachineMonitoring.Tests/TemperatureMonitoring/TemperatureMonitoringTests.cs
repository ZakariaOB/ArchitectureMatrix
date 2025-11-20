using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Services;
using Moq;

namespace HexagonalMachineMonitoring.Tests.TemperatureMonitoring
{
    public class TemperatureMonitoringTests
    {
        [Fact]
        public async Task Should_Send_Alert_When_Threshold_Exceeded()
        {
            var inbound = new Mock<IGetTemperaturePort>();
            inbound.Setup(s => s.GetLatestAsync(5))
                   .ReturnsAsync(new TemperatureReading(5, 90, DateTime.UtcNow));

            var alert = new Mock<ISendAlertPort>();
            var store = new Mock<IStoreTemperaturePort>();

            var service = new TemperatureMonitoringService(
                inbound.Object,
                alert.Object,
                store.Object,
                threshold: 50);

            await service.MonitorAsync(5);

            alert.Verify(a => a.SendAsync(5, 90), Times.Once);
            store.Verify(s => s.SaveAsync(It.IsAny<TemperatureReading>()), Times.Once);
        }

        [Fact]
        public async Task Should_Not_Send_Alert_When_Threshold_Not_Exceeded()
        {
            var inbound = new Mock<IGetTemperaturePort>();
            inbound.Setup(s => s.GetLatestAsync(3))
                   .ReturnsAsync(new TemperatureReading(3, 40, DateTime.UtcNow));

            var alert = new Mock<ISendAlertPort>();
            var store = new Mock<IStoreTemperaturePort>();

            var service = new TemperatureMonitoringService(
                inbound.Object,
                alert.Object,
                store.Object,
                threshold: 50);

            await service.MonitorAsync(3);

            alert.Verify(a => a.SendAsync(It.IsAny<int>(), It.IsAny<double>()), Times.Never);
            store.Verify(s => s.SaveAsync(It.IsAny<TemperatureReading>()), Times.Once);
        }
    }
}

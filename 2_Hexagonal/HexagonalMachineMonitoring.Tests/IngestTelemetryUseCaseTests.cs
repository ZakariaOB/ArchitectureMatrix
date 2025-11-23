using Application.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Outbound;
using HexagonalMachineMonitoring.Core.UseCases;
using Moq;

namespace HexagonalMachineMonitoring.Tests;

public class IngestTelemetryUseCaseTests
{
    [Fact]
    public async Task Should_Store_And_Notify_When_Anomaly()
    {
        var store = new Mock<IStoreTelemetryPort>();
        var anomaly = new Mock<IRunAnomalyDetectionPort>();
        var notify = new Mock<ISendNotificationPort>();
        var events = new Mock<IPublishEventPort>();

        var reading = new TelemetryReading(1, 100, 0.5, DateTime.UtcNow);

        anomaly
            .Setup(a => a.CheckAsync(reading, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnomalyResult(true, "Overheat", 0.9));

        var useCase = new IngestTelemetryUseCase(store.Object, anomaly.Object, notify.Object, events.Object);

        await useCase.HandleAsync(reading);

        store.Verify(s => s.SaveAsync(reading, It.IsAny<CancellationToken>()), Times.Once);
        notify.Verify(n => n.SendAsync(1, "Overheat", 0.9, It.IsAny<CancellationToken>()), Times.Once);
        events.Verify(e => e.PublishAsync("Machine.AnomalyDetected", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Store_But_Not_Notify_When_No_Anomaly()
    {
        var store = new Mock<IStoreTelemetryPort>();
        var anomaly = new Mock<IRunAnomalyDetectionPort>();
        var notify = new Mock<ISendNotificationPort>();
        var events = new Mock<IPublishEventPort>();

        var reading = new TelemetryReading(1, 60, 0.5, DateTime.UtcNow);

        anomaly
            .Setup(a => a.CheckAsync(reading, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnomalyResult(false, string.Empty, 0));

        var useCase = new IngestTelemetryUseCase(store.Object, anomaly.Object, notify.Object, events.Object);

        await useCase.HandleAsync(reading);

        store.Verify(s => s.SaveAsync(reading, It.IsAny<CancellationToken>()), Times.Once);
        notify.VerifyNoOtherCalls();
        events.VerifyNoOtherCalls();
    }
}

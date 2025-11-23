using Application.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Core.UseCases;

public sealed class IngestTelemetryUseCase(
    IStoreTelemetryPort store,
    IRunAnomalyDetectionPort anomaly,
    ISendNotificationPort notify,
    IPublishEventPort events) : IIngestTelemetryUseCase
{
    private readonly IStoreTelemetryPort _store = store;
    private readonly IRunAnomalyDetectionPort _anomaly = anomaly;
    private readonly ISendNotificationPort _notify = notify;
    private readonly IPublishEventPort _events = events;

    public async Task HandleAsync(TelemetryReading reading, CancellationToken ct = default)
    {
        // 1. Persist the reading
        await _store.SaveAsync(reading, ct);

        // 2. Run anomaly detection
        var result = await _anomaly.CheckAsync(reading, ct);

        if (!result.IsAnomaly)
        {
            return;
        }

        // 3. Send notification
        await _notify.SendAsync(reading.MachineId, result.Reason, result.Severity, ct);

        // 4. Publish domain event
        await _events.PublishAsync("Machine.AnomalyDetected", result, ct);
    }
}

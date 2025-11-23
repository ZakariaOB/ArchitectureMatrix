using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface IRunAnomalyDetectionPort
{
    Task<AnomalyResult> CheckAsync(
        TelemetryReading reading, 
        CancellationToken ct = default);
}

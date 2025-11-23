using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Core.Ports.Inbound;

public interface IIngestTelemetryUseCase
{
    Task HandleAsync(TelemetryReading reading, CancellationToken ct = default);
}
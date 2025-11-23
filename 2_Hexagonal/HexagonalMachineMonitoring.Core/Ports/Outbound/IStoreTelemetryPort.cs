using HexagonalMachineMonitoring.Core.Domain.Models;

namespace Application.Ports.Outbound;

public interface IStoreTelemetryPort
{
    Task SaveAsync(
        TelemetryReading reading, 
        CancellationToken ct = default);
}
using Application.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound;

public sealed class SqlTelemetryStoreAdapter : IStoreTelemetryPort
{
    // In real life inject DbContext or IDbConnection
    public Task SaveAsync(TelemetryReading reading, CancellationToken ct = default)
    {
        Console.WriteLine($"[SQL] Saved telemetry for machine {reading.MachineId} ({reading.TemperatureCelsius}°C).");
        return Task.CompletedTask;
    }
}
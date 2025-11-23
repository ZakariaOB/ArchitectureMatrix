namespace HexagonalMachineMonitoring.Core.Domain.Models;

public sealed record TelemetryReading(
    int MachineId,
    double TemperatureCelsius,
    double LoadPercentage,
    DateTime TimestampUtc);

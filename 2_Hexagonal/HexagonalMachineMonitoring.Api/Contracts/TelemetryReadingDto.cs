namespace HexagonalMachineMonitoring.Api.Contracts;

public sealed record TelemetryReadingDto(
    int MachineId,
    double TemperatureCelsius,
    double LoadPercentage,
    DateTime TimestampUtc);
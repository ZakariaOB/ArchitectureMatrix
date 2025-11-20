namespace HexagonalMachineMonitoring.Core.Domain.Models;

public record TemperatureReading(
    int MachineId,
    double Value,
    DateTime Timestamp);

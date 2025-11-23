using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Core.Domain.Models.Rules;

public static class AnomalyRules
{
    // Very simple rule just for the example
    public static AnomalyResult EvaluateTemperature(TelemetryReading reading, double threshold)
    {
        if (reading.TemperatureCelsius <= threshold)
        {
            return new AnomalyResult(false, string.Empty, 0);
        }

        var severity = Math.Min(1.0, (reading.TemperatureCelsius - threshold) / 50.0);
        var reason = $"Temperature {reading.TemperatureCelsius:F1}°C exceeds threshold {threshold:F1}°C";

        return new AnomalyResult(true, reason, severity);
    }
}
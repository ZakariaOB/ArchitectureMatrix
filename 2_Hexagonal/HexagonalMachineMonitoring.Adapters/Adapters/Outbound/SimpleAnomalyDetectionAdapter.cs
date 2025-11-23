using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Domain.Models.Rules;
using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace Adapters.Outbound;

public sealed class SimpleAnomalyDetectionAdapter(double threshold = 80.0) 
    : IRunAnomalyDetectionPort
{
    private readonly double _threshold = threshold;

    public Task<AnomalyResult> CheckAsync(TelemetryReading reading, CancellationToken ct = default)
    {
        var result = AnomalyRules.EvaluateTemperature(reading, _threshold);
        return Task.FromResult(result);
    }
}
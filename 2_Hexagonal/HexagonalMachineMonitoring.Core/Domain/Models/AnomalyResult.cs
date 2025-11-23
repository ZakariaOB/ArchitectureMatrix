namespace HexagonalMachineMonitoring.Core.Domain.Models;

public sealed record AnomalyResult(
    bool IsAnomaly,
    string Reason,
    double Severity);
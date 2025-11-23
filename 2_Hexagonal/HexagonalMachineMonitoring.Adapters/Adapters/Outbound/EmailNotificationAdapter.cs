using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound;

public sealed class EmailNotificationAdapter : ISendNotificationPort
{
    public Task SendAsync(int machineId, string message, double severity, CancellationToken ct = default)
    {
        Console.WriteLine($"[EMAIL] Machine {machineId}: {message} (severity={severity:P0})");
        return Task.CompletedTask;
    }
}
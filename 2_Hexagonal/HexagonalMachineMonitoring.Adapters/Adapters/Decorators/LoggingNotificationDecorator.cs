using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace Adapters.Decorators;

public sealed class LoggingNotificationDecorator(ISendNotificationPort inner) : ISendNotificationPort
{
    private readonly ISendNotificationPort _inner = inner;

    public async Task SendAsync(
        int machineId, 
        string message, 
        double severity, 
        CancellationToken ct = default)
    {
        Console.WriteLine($"[LOG] Sending notification for machine {machineId}: {message}");
        await _inner.SendAsync(machineId, message, severity, ct);
    }
}
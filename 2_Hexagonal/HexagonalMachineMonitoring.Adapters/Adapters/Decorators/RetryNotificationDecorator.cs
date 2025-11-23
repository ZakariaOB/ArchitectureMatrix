using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Decorators;

public sealed class RetryNotificationDecorator(
    ISendNotificationPort inner, int retries = 3) : ISendNotificationPort
{
    private readonly ISendNotificationPort _inner = inner;
    private readonly int _retries = retries;

    public async Task SendAsync(
        int machineId, 
        string message, 
        double severity, 
        CancellationToken ct = default)
    {
        for (var attempt = 0; attempt <= _retries; attempt++)
        {
            try
            {
                await _inner.SendAsync(machineId, message, severity, ct);
                return;
            }
            catch when (attempt < _retries)
            {
                Console.WriteLine($"[RETRY] Notification failed, retry {attempt + 1}/{_retries}.");
                // small delay in real life
            }
        }
    }
}
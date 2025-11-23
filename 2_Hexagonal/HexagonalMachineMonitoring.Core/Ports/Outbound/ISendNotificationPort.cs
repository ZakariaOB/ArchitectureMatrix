namespace HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface ISendNotificationPort
{
    Task SendAsync(
        int machineId, 
        string message, 
        double severity, 
        CancellationToken ct = default);
}

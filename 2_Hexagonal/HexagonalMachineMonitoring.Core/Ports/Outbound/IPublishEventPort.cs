namespace HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface IPublishEventPort
{
    Task PublishAsync(
        string eventName, 
        object payload, 
        CancellationToken ct = default);
}

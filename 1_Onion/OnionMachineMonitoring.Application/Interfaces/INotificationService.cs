namespace MachineMonitoring.Application;

/// <summary>Outbound notification from the use case.</summary>
public interface INotificationService
{
    Task PublishProdSyncedAsync(int count, object range);
}

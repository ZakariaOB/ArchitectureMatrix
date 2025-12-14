using MachineMonitoringClean10.Core.MachineAggregate.Events;

namespace MachineMonitoringClean10.Core.MachineAggregate.Handlers;

/// <summary>
/// Handler for MachineDeletedEvent.
/// Logs machine deletion and could trigger cleanup operations.
/// </summary>
public class MachineDeletedHandler(ILogger<MachineDeletedHandler> logger) 
  : INotificationHandler<MachineDeletedEvent>
{
  public ValueTask Handle(MachineDeletedEvent notification, CancellationToken cancellationToken)
  {
    logger.LogInformation(
      "Machine deleted: {MachineName} (ID: {MachineId})",
      notification.MachineName,
      notification.MachineId);

    // In a real application:
    // - Archive machine data
    // - Notify relevant parties
    // - Clean up related resources
    // - Update dashboards

    return ValueTask.CompletedTask;
  }
}

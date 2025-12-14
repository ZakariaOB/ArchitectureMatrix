using MachineMonitoringClean10.Core.MachineAggregate.Events;

namespace MachineMonitoringClean10.Core.MachineAggregate.Handlers;

/// <summary>
/// Handler for MachineProductionAddedEvent.
/// Could trigger alerts for low/high production, update dashboards, etc.
/// </summary>
public class MachineProductionAddedHandler(
  ILogger<MachineProductionAddedHandler> logger) : INotificationHandler<MachineProductionAddedEvent>
{
  public ValueTask Handle(MachineProductionAddedEvent notification, CancellationToken cancellationToken)
  {
    logger.LogInformation(
      "Production recorded for machine {MachineName}: {Quantity} units (Batch: {BatchNumber})",
      notification.Machine.Name,
      notification.Production.Quantity,
      notification.Production.BatchNumber ?? "N/A");

    // In a real application:
    // - Update real-time dashboards
    // - Check against production targets
    // - Trigger alerts for anomalies

    return ValueTask.CompletedTask;
  }
}

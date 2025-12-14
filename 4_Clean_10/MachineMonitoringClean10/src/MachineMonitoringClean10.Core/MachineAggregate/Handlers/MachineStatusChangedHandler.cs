using MachineMonitoringClean10.Core.Interfaces;
using MachineMonitoringClean10.Core.MachineAggregate.Events;

namespace MachineMonitoringClean10.Core.MachineAggregate.Handlers;

/// <summary>
/// Handler for MachineStatusChangedEvent.
/// Notifies operators and updates monitoring systems when machine status changes.
/// </summary>
public class MachineStatusChangedHandler(
  ILogger<MachineStatusChangedHandler> logger,
  IEmailSender emailSender) : INotificationHandler<MachineStatusChangedEvent>
{
  public async ValueTask Handle(MachineStatusChangedEvent notification, CancellationToken cancellationToken)
  {
    logger.LogInformation(
      "Machine status changed: {MachineName} from {PreviousStatus} to {NewStatus}",
      notification.Machine.Name,
      notification.PreviousStatus.Name,
      notification.NewStatus.Name);

    // Send notification for important status changes
    if (notification.NewStatus == MachineStatus.Maintenance || 
        notification.NewStatus == MachineStatus.Decommissioned)
    {
      await emailSender.SendEmailAsync(
        "operations@factory.com",
        "operations@factory.com",
        $"Machine Status Changed: {notification.Machine.Name}",
        $"Machine '{notification.Machine.Name}' status changed from {notification.PreviousStatus.Name} to {notification.NewStatus.Name}.");
    }

    // In a real application:
    // - Update real-time dashboards
    // - Trigger maintenance workflows
    // - Notify production planning team
    // - Update capacity calculations
  }
}

using MachineMonitoringClean10.Core.Interfaces;
using MachineMonitoringClean10.Core.MachineAggregate.Events;

namespace MachineMonitoringClean10.Core.MachineAggregate.Handlers;

/// <summary>
/// Handler for MachineCreatedEvent.
/// Demonstrates Clean Architecture's use of domain event handlers for side effects.
/// </summary>
public class MachineCreatedNotificationHandler(
  ILogger<MachineCreatedNotificationHandler> logger,
  IEmailSender emailSender) : INotificationHandler<MachineCreatedEvent>
{
  public async ValueTask Handle(MachineCreatedEvent notification, CancellationToken cancellationToken)
  {
    logger.LogInformation("Machine created: {MachineName} (ID: {MachineId})",
      notification.Machine.Name,
      notification.Machine.Id);

    // In a real application, you might notify operators or update dashboards
    await emailSender.SendEmailAsync(
      "admin@factory.com",
      "admin@factory.com",
      "New Machine Registered",
      $"A new machine '{notification.Machine.Name}' has been registered in the system.");
  }
}

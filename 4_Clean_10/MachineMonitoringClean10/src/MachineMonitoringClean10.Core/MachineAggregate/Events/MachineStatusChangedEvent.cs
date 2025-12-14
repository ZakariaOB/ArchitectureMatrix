namespace MachineMonitoringClean10.Core.MachineAggregate.Events;

/// <summary>
/// Domain Event raised when a machine's status changes.
/// </summary>
public sealed class MachineStatusChangedEvent(Machine machine, MachineStatus previousStatus) : DomainEventBase
{
  public Machine Machine { get; } = machine;
  public MachineStatus PreviousStatus { get; } = previousStatus;
  public MachineStatus NewStatus => Machine.Status;
}

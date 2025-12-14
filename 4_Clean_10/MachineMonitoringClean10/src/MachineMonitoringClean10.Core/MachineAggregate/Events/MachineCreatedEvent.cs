namespace MachineMonitoringClean10.Core.MachineAggregate.Events;

/// <summary>
/// Domain Event raised when a new machine is created.
/// Clean Architecture: Domain events enable loose coupling between aggregates.
/// </summary>
public sealed class MachineCreatedEvent(Machine machine) : DomainEventBase
{
  public Machine Machine { get; } = machine;
}

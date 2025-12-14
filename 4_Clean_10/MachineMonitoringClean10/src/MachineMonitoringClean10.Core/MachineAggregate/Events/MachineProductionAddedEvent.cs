namespace MachineMonitoringClean10.Core.MachineAggregate.Events;

/// <summary>
/// Domain Event raised when production is recorded for a machine.
/// Demonstrates Clean Architecture's event-driven domain model.
/// </summary>
public sealed class MachineProductionAddedEvent(Machine machine, MachineProduction production) : DomainEventBase
{
  public Machine Machine { get; } = machine;
  public MachineProduction Production { get; } = production;
}

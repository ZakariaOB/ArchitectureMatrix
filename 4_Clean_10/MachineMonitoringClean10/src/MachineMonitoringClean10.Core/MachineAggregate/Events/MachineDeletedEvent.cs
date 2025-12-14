namespace MachineMonitoringClean10.Core.MachineAggregate.Events;

/// <summary>
/// Domain Event raised when a machine is deleted.
/// </summary>
public sealed class MachineDeletedEvent(MachineId machineId, MachineName machineName) : DomainEventBase
{
  public MachineId MachineId { get; } = machineId;
  public MachineName MachineName { get; } = machineName;
}

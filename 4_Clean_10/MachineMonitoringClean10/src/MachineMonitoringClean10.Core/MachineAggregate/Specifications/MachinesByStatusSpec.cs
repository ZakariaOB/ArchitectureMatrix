namespace MachineMonitoringClean10.Core.MachineAggregate.Specifications;

/// <summary>
/// Specification to find machines by status.
/// </summary>
public sealed class MachinesByStatusSpec : Specification<Machine>
{
  public MachinesByStatusSpec(MachineStatus status)
  {
    Query
      .Where(m => m.Status == status)
      .OrderBy(m => m.Name);
  }
}

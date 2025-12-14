namespace MachineMonitoringClean10.Core.MachineAggregate.Specifications;

/// <summary>
/// Specification to find a Machine by its ID.
/// Clean Architecture: Specifications encapsulate query logic in the domain.
/// </summary>
public sealed class MachineByIdSpec : Specification<Machine>, ISingleResultSpecification<Machine>
{
  public MachineByIdSpec(MachineId machineId)
  {
    Query
      .Where(m => m.Id == machineId);
  }
}

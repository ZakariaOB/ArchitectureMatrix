namespace MachineMonitoringClean10.UseCases.Machines.List;

/// <summary>
/// Interface for listing machines query service.
/// Clean Architecture: Interface defined in UseCases, implemented in Infrastructure.
/// This enables optimized read queries that bypass the domain model.
/// </summary>
public interface IListMachinesQueryService
{
  Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(int? skip, int? take, CancellationToken cancellationToken);
}

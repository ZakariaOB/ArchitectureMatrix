namespace MachineMonitoringClean10.UseCases.Machines.List;

/// <summary>
/// Handler for ListMachinesQuery.
/// Uses a dedicated query service for optimized read operations (CQRS pattern).
/// </summary>
public class ListMachinesHandler(IListMachinesQueryService queryService)
  : IQueryHandler<ListMachinesQuery, Result<(IReadOnlyList<MachineDTO> Items, int TotalCount)>>
{
  public async ValueTask<Result<(IReadOnlyList<MachineDTO> Items, int TotalCount)>> Handle(
    ListMachinesQuery query,
    CancellationToken cancellationToken)
  {
    var result = await queryService.ListAsync(query.Skip, query.Take, cancellationToken);
    return result;
  }
}

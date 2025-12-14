using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.UseCases.Machines;
using MachineMonitoringClean10.UseCases.Machines.List;

namespace MachineMonitoringClean10.Infrastructure.Data.Queries;

/// <summary>
/// Query service for listing machines.
/// Clean Architecture: Read queries can bypass the domain model for performance.
/// This service is defined in Infrastructure but implements an interface from UseCases.
/// </summary>
public class ListMachinesQueryService(AppDbContext dbContext) : IListMachinesQueryService
{
  public async Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(
    int? skip,
    int? take,
    CancellationToken cancellationToken)
  {
    var query = dbContext.Machines.AsNoTracking();

    var totalCount = await query.CountAsync(cancellationToken);

    var machines = await query
      .OrderBy(m => m.Name)
      .Skip(skip ?? 0)
      .Take(take ?? 10)
      .Select(m => new MachineDTO(
        m.Id.Value,
        m.Name.Value,
        m.Description,
        m.Status.Name,
        m.CreatedAt,
        m.LastProductionAt,
        m.Productions.Sum(p => p.Quantity)))
      .ToListAsync(cancellationToken);

    return (machines, totalCount);
  }
}

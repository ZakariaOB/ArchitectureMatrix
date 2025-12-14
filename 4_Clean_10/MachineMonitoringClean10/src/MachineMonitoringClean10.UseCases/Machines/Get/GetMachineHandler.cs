using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.Core.MachineAggregate.Specifications;

namespace MachineMonitoringClean10.UseCases.Machines.Get;

/// <summary>
/// Handler for GetMachineQuery.
/// Uses Specification pattern to encapsulate query logic.
/// </summary>
public class GetMachineHandler(IReadRepository<Machine> repository)
  : IQueryHandler<GetMachineQuery, Result<MachineDTO>>
{
  public async ValueTask<Result<MachineDTO>> Handle(
    GetMachineQuery query,
    CancellationToken cancellationToken)
  {
    var machineId = MachineId.From(query.MachineId);
    var spec = new MachineByIdSpec(machineId);

    var machine = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (machine is null)
    {
      return Result<MachineDTO>.NotFound();
    }

    return new MachineDTO(
      machine.Id.Value,
      machine.Name.Value,
      machine.Description,
      machine.Status.Name,
      machine.CreatedAt,
      machine.LastProductionAt,
      machine.GetTotalProduction());
  }
}

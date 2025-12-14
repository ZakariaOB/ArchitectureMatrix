using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.Core.MachineAggregate.Specifications;

namespace MachineMonitoringClean10.UseCases.Machines.AddProduction;

/// <summary>
/// Handler for AddProductionCommand.
/// 
/// Clean Architecture Strength: Business rules (only active machines can record production)
/// are enforced in the domain entity, not in the use case handler.
/// </summary>
public class AddProductionHandler(IRepository<Machine> repository)
  : ICommandHandler<AddProductionCommand, Result>
{
  public async ValueTask<Result> Handle(
    AddProductionCommand command,
    CancellationToken cancellationToken)
  {
    var machineId = MachineId.From(command.MachineId);
    var spec = new MachineByIdSpec(machineId);

    var machine = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (machine is null)
    {
      return Result.NotFound("Machine not found");
    }

    // Domain logic: The Machine entity enforces that only active machines can record production
    // This throws InvalidOperationException if machine is not active
    machine.AddProduction(command.Quantity, command.ProducedAt, command.BatchNumber);

    await repository.UpdateAsync(machine, cancellationToken);

    return Result.Success();
  }
}

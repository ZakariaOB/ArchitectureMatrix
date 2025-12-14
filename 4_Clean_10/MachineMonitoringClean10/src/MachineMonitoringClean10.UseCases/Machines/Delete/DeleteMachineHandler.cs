using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.Core.MachineAggregate.Specifications;

namespace MachineMonitoringClean10.UseCases.Machines.Delete;

/// <summary>
/// Handler for DeleteMachineCommand.
/// 
/// Clean Architecture Strength: The domain entity (Machine) enforces deletion rules,
/// not the use case handler. This ensures business rules are consistently applied.
/// </summary>
public class DeleteMachineHandler(IRepository<Machine> repository)
  : ICommandHandler<DeleteMachineCommand, Result>
{
  public async ValueTask<Result> Handle(
    DeleteMachineCommand command,
    CancellationToken cancellationToken)
  {
    var machineId = MachineId.From(command.MachineId);
    var spec = new MachineByIdSpec(machineId);

    var machine = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (machine is null)
    {
      return Result.NotFound("Machine not found");
    }

    // Domain validation: Machine.EnsureCanBeDeleted() throws if:
    // - Machine is in Active or Maintenance status
    // - Machine has production records in the last 30 days
    machine.EnsureCanBeDeleted();

    await repository.DeleteAsync(machine, cancellationToken);

    return Result.Success();
  }
}

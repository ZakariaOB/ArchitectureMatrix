using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.Core.MachineAggregate.Specifications;

namespace MachineMonitoringClean10.UseCases.Machines.UpdateStatus;

/// <summary>
/// Handler for UpdateMachineStatusCommand.
/// 
/// Clean Architecture Strength: Status transition rules are encapsulated in the Machine entity.
/// The handler orchestrates the operation but delegates business logic to the domain.
/// </summary>
public class UpdateMachineStatusHandler(IRepository<Machine> repository)
  : ICommandHandler<UpdateMachineStatusCommand, Result>
{
  public async ValueTask<Result> Handle(
    UpdateMachineStatusCommand command,
    CancellationToken cancellationToken)
  {
    var machineId = MachineId.From(command.MachineId);
    var spec = new MachineByIdSpec(machineId);

    var machine = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (machine is null)
    {
      return Result.NotFound("Machine not found");
    }

    // Parse the status using SmartEnum
    if (!MachineStatus.TryFromName(command.NewStatus, ignoreCase: true, out var newStatus))
    {
      return Result.Invalid(new ValidationError($"Invalid status: {command.NewStatus}. Valid values: Active, Inactive, Maintenance, Decommissioned"));
    }

    // Apply status change - domain entity enforces valid transitions
    _ = newStatus.Name switch
    {
      nameof(MachineStatus.Active) => machine.Activate(),
      nameof(MachineStatus.Inactive) => machine.Deactivate(),
      nameof(MachineStatus.Maintenance) => machine.SetMaintenance(),
      nameof(MachineStatus.Decommissioned) => machine.Decommission(),
      _ => throw new InvalidOperationException($"Unknown status: {newStatus.Name}")
    };

    await repository.UpdateAsync(machine, cancellationToken);

    return Result.Success();
  }
}

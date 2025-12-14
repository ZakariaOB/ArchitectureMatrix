using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.Create;

/// <summary>
/// Handler for CreateMachineCommand.
/// Clean Architecture: Use Cases orchestrate domain operations without infrastructure concerns.
/// </summary>
public class CreateMachineHandler(IRepository<Machine> repository)
  : ICommandHandler<CreateMachineCommand, Result<MachineId>>
{
  public async ValueTask<Result<MachineId>> Handle(
    CreateMachineCommand command,
    CancellationToken cancellationToken)
  {
    // Create value object - validation happens in the domain layer
    // MachineName.From() will throw if validation fails (Parse, Don't Validate principle)
    var machineName = MachineName.From(command.Name);

    var machine = new Machine(machineName, command.Description);

    var createdMachine = await repository.AddAsync(machine, cancellationToken);

    return createdMachine.Id;
  }
}

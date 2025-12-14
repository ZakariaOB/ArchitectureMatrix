using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.Create;

/// <summary>
/// Command to create a new machine.
/// CQRS pattern: Commands represent intent to change state.
/// </summary>
public record CreateMachineCommand(
  string Name,
  string? Description) : ICommand<Result<MachineId>>;

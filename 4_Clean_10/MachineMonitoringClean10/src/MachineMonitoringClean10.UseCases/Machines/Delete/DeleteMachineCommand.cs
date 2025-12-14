using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.Delete;

/// <summary>
/// Command to delete a machine.
/// </summary>
public record DeleteMachineCommand(int MachineId) : ICommand<Result>;

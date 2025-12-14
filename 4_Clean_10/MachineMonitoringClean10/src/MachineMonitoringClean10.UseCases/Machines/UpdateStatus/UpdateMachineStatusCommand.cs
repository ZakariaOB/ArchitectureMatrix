using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.UpdateStatus;

/// <summary>
/// Command to update machine status.
/// </summary>
public record UpdateMachineStatusCommand(
  int MachineId,
  string NewStatus) : ICommand<Result>;

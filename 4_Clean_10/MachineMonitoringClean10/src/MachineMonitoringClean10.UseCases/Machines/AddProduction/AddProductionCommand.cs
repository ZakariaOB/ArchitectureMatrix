using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.AddProduction;

/// <summary>
/// Command to add production to a machine.
/// Demonstrates a domain-rich operation beyond simple CRUD.
/// </summary>
public record AddProductionCommand(
  int MachineId,
  int Quantity,
  DateTime ProducedAt,
  string? BatchNumber) : ICommand<Result>;

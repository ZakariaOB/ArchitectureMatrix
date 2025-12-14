using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines.Get;

/// <summary>
/// Query to get a machine by ID.
/// CQRS pattern: Queries fetch data without side effects.
/// </summary>
public record GetMachineQuery(int MachineId) : IQuery<Result<MachineDTO>>;

namespace MachineMonitoringClean10.UseCases.Machines.List;

/// <summary>
/// Query to list all machines with pagination.
/// </summary>
public record ListMachinesQuery(int? Skip, int? Take) : IQuery<Result<(IReadOnlyList<MachineDTO> Items, int TotalCount)>>;

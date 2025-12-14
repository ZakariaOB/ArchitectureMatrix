using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.UseCases.Machines;

/// <summary>
/// DTO for Machine data transfer.
/// Clean Architecture: DTOs in UseCases layer decouple domain from presentation.
/// </summary>
public record MachineDTO(
  int Id,
  string Name,
  string? Description,
  string Status,
  DateTime CreatedAt,
  DateTime? LastProductionAt,
  int TotalProduction);

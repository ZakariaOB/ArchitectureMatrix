namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Record representation of a Machine for API responses.
/// </summary>
public record MachineRecord(
  int Id,
  string Name,
  string? Description,
  string Status,
  DateTime CreatedAt,
  DateTime? LastProductionAt,
  int TotalProduction);

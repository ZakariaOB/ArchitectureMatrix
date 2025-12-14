namespace MachineMonitoringClean10.Core.MachineAggregate;

/// <summary>
/// SmartEnum for Machine status.
/// Clean Architecture: Business rules are encoded in the domain using rich types.
/// </summary>
public class MachineStatus : SmartEnum<MachineStatus>
{
  public static readonly MachineStatus Inactive = new(nameof(Inactive), 0);
  public static readonly MachineStatus Active = new(nameof(Active), 1);
  public static readonly MachineStatus Maintenance = new(nameof(Maintenance), 2);
  public static readonly MachineStatus Decommissioned = new(nameof(Decommissioned), 3);

  private MachineStatus(string name, int value) : base(name, value) { }

  /// <summary>
  /// Determines if the machine can record production in this status.
  /// </summary>
  public bool CanRecordProduction => this == Active;

  /// <summary>
  /// Determines if the machine can be deleted in this status.
  /// </summary>
  public bool CanBeDeleted => this == Inactive || this == Decommissioned;
}

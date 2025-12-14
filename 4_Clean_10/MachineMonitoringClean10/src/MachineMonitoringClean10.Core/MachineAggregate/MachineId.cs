using Vogen;

namespace MachineMonitoringClean10.Core.MachineAggregate;

/// <summary>
/// Strongly-typed ID for Machine aggregate.
/// Demonstrates Clean Architecture's use of Value Objects to eliminate primitive obsession.
/// </summary>
[ValueObject<int>]
public readonly partial struct MachineId
{
  private static Validation Validate(int value)
      => value > 0 ? Validation.Ok : Validation.Invalid("MachineId must be positive.");
}

namespace MachineMonitoringClean10.Core.MachineAggregate;

/// <summary>
/// Value Object representing a production record for a machine.
/// Immutable and validated - Clean Architecture principle of rich domain models.
/// </summary>
public sealed record MachineProduction
{
  public int Quantity { get; }
  public DateTime ProducedAt { get; }
  public DateTime RecordedAt { get; }
  public string? BatchNumber { get; }

  public MachineProduction(int quantity, DateTime producedAt, DateTime recordedAt, string? batchNumber = null)
  {
    Guard.Against.NegativeOrZero(quantity, nameof(quantity), "Production quantity must be positive");
    Guard.Against.Default(producedAt, nameof(producedAt));
    Guard.Against.Default(recordedAt, nameof(recordedAt));

    if (producedAt > recordedAt)
      throw new ArgumentException("Production date cannot be in the future relative to recording date");

    Quantity = quantity;
    ProducedAt = producedAt;
    RecordedAt = recordedAt;
    BatchNumber = batchNumber;
  }
}

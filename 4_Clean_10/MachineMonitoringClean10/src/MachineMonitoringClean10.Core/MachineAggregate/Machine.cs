using MachineMonitoringClean10.Core.MachineAggregate.Events;

namespace MachineMonitoringClean10.Core.MachineAggregate;

/// <summary>
/// Machine Aggregate Root - the central entity for machine monitoring.
/// 
/// Clean Architecture Strengths Demonstrated:
/// 1. Rich Domain Model - Business logic is encapsulated in the entity
/// 2. Value Objects - MachineName, MachineId prevent primitive obsession
/// 3. Domain Events - Enable loose coupling and side effects
/// 4. Encapsulation - Private setters protect invariants
/// 5. Guard Clauses - Validate business rules at domain level
/// 6. Aggregate Pattern - Machine owns its productions
/// </summary>
public class Machine : EntityBase<Machine, MachineId>, IAggregateRoot
{
  public MachineName Name { get; private set; }
  public string? Description { get; private set; }
  public MachineStatus Status { get; private set; } = MachineStatus.Inactive;
  public DateTime CreatedAt { get; private set; }
  public DateTime? LastProductionAt { get; private set; }

  private readonly List<MachineProduction> _productions = [];
  public IReadOnlyCollection<MachineProduction> Productions => _productions.AsReadOnly();

  // Required for EF Core
  private Machine() { }

  /// <summary>
  /// Creates a new Machine with the specified name.
  /// Factory method ensures the aggregate is always in a valid state.
  /// </summary>
  public Machine(MachineName name, string? description = null)
  {
    Name = name;
    Description = description;
    CreatedAt = DateTime.UtcNow;
    Status = MachineStatus.Inactive;

    RegisterDomainEvent(new MachineCreatedEvent(this));
  }

  /// <summary>
  /// Activates the machine for production.
  /// Business rule: Only inactive machines can be activated.
  /// </summary>
  public Machine Activate()
  {
    if (Status == MachineStatus.Active)
      return this;

    if (Status == MachineStatus.Decommissioned)
      throw new InvalidOperationException("Cannot activate a decommissioned machine");

    var previousStatus = Status;
    Status = MachineStatus.Active;
    RegisterDomainEvent(new MachineStatusChangedEvent(this, previousStatus));

    return this;
  }

  /// <summary>
  /// Sets the machine to maintenance mode.
  /// </summary>
  public Machine SetMaintenance()
  {
    if (Status == MachineStatus.Maintenance)
      return this;

    if (Status == MachineStatus.Decommissioned)
      throw new InvalidOperationException("Cannot set maintenance on a decommissioned machine");

    var previousStatus = Status;
    Status = MachineStatus.Maintenance;
    RegisterDomainEvent(new MachineStatusChangedEvent(this, previousStatus));

    return this;
  }

  /// <summary>
  /// Deactivates the machine.
  /// </summary>
  public Machine Deactivate()
  {
    if (Status == MachineStatus.Inactive)
      return this;

    if (Status == MachineStatus.Decommissioned)
      throw new InvalidOperationException("Cannot deactivate a decommissioned machine");

    var previousStatus = Status;
    Status = MachineStatus.Inactive;
    RegisterDomainEvent(new MachineStatusChangedEvent(this, previousStatus));

    return this;
  }

  /// <summary>
  /// Permanently decommissions the machine.
  /// Business rule: Decommissioned machines cannot be reactivated.
  /// </summary>
  public Machine Decommission()
  {
    if (Status == MachineStatus.Decommissioned)
      return this;

    var previousStatus = Status;
    Status = MachineStatus.Decommissioned;
    RegisterDomainEvent(new MachineStatusChangedEvent(this, previousStatus));

    return this;
  }

  /// <summary>
  /// Records production for this machine.
  /// Business rule: Only active machines can record production.
  /// </summary>
  public Machine AddProduction(int quantity, DateTime producedAt, string? batchNumber = null)
  {
    if (!Status.CanRecordProduction)
      throw new InvalidOperationException($"Cannot record production for a machine in {Status.Name} status");

    var production = new MachineProduction(quantity, producedAt, DateTime.UtcNow, batchNumber);
    _productions.Add(production);
    LastProductionAt = producedAt;

    RegisterDomainEvent(new MachineProductionAddedEvent(this, production));

    return this;
  }

  /// <summary>
  /// Gets total production quantity for this machine.
  /// </summary>
  public int GetTotalProduction() => _productions.Sum(p => p.Quantity);

  /// <summary>
  /// Gets production within a date range.
  /// </summary>
  public int GetProductionBetween(DateTime start, DateTime end) =>
    _productions
      .Where(p => p.ProducedAt >= start && p.ProducedAt <= end)
      .Sum(p => p.Quantity);

  /// <summary>
  /// Checks if machine has recent production (within specified days).
  /// Used for deletion rules.
  /// </summary>
  public bool HasRecentProduction(int days = 30) =>
    _productions.Any(p => p.ProducedAt >= DateTime.UtcNow.AddDays(-days));

  /// <summary>
  /// Validates if the machine can be deleted.
  /// Business rules:
  /// - Must be in deletable status (Inactive or Decommissioned)
  /// - Must not have recent production
  /// </summary>
  public void EnsureCanBeDeleted()
  {
    if (!Status.CanBeDeleted)
      throw new InvalidOperationException($"Machine in {Status.Name} status cannot be deleted. Deactivate or decommission first.");

    if (HasRecentProduction())
      throw new InvalidOperationException("Cannot delete machine with production records in the last 30 days");
  }

  /// <summary>
  /// Updates the machine name.
  /// </summary>
  public Machine UpdateName(MachineName newName)
  {
    if (Name == newName) return this;
    Name = newName;
    return this;
  }

  /// <summary>
  /// Updates the machine description.
  /// </summary>
  public Machine UpdateDescription(string? description)
  {
    Description = description;
    return this;
  }
}

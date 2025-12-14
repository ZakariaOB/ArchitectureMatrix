# Clean Architecture in Machine Monitoring - .NET 10 Implementation

## Overview

This document explains the Machine Monitoring implementation in the **Clean Architecture** pattern using .NET 10, highlighting the **strengths and advantages** of this architectural approach compared to Layered, Onion, and Hexagonal architectures.

---

## ?? Clean Architecture Structure

```
???????????????????????????????????????????????????????????
?                      Presentation                        ?
?              (Web API - FastEndpoints)                   ?
?  Controllers, Validators, HTTP concerns                  ?
???????????????????????????????????????????????????????????
                        ? Depends on
???????????????????????????????????????????????????????????
?                     Use Cases                            ?
?         (Application Business Rules)                     ?
?  Commands, Queries, Handlers, DTOs                       ?
???????????????????????????????????????????????????????????
                        ? Depends on
???????????????????????????????????????????????????????????
?                       Core                               ?
?            (Enterprise Business Rules)                   ?
?  Entities, Value Objects, Domain Events                  ?
?  Aggregates, Specifications, Interfaces                  ?
???????????????????????????????????????????????????????????
                        ?
                        ? Implements
???????????????????????????????????????????????????????????
?                 Infrastructure                           ?
?       (External Concerns - Plugins)                      ?
?  Database, External APIs, Email, File System             ?
???????????????????????????????????????????????????????????
```

**Key Principle**: Dependencies point **inward**. Core has **zero dependencies** on outer layers.

---

## ?? Clean Architecture Strengths Demonstrated

### 1. **Independence from Frameworks**

**Example**: The `Machine` entity doesn't depend on Entity Framework, Web frameworks, or any infrastructure:

```csharp
/// <summary>
/// Machine Aggregate Root - Pure domain logic, no framework dependencies
/// </summary>
public class Machine : EntityBase<Machine, MachineId>, IAggregateRoot
{
    public MachineName Name { get; private set; }
    public MachineStatus Status { get; private set; } = MachineStatus.Inactive;
    private readonly List<MachineProduction> _productions = [];
    
    // Business rules enforced in the domain
    public Machine AddProduction(int quantity, DateTime producedAt, string? batchNumber = null)
    {
        if (!Status.CanRecordProduction)
            throw new InvalidOperationException(
                $"Cannot record production for a machine in {Status.Name} status");
        
        var production = new MachineProduction(quantity, producedAt, DateTime.UtcNow, batchNumber);
        _productions.Add(production);
        
        RegisterDomainEvent(new MachineProductionAddedEvent(this, production));
        return this;
    }
}
```

**Why this matters**:
- ? You can switch from EF Core to Dapper or any ORM without touching domain logic
- ? Web framework changes (ASP.NET Core ? minimal APIs ? gRPC) don't affect core business rules
- ? Domain logic can be tested without any infrastructure

### 2. **Testability**

**Example**: Testing `AddProduction` without any database or infrastructure:

```csharp
[Fact]
public void AddProduction_WhenMachineInactive_ThrowsException()
{
    // Arrange - Pure domain objects
    var machine = new Machine(MachineName.From("CNC-001"));
    
    // Act & Assert - No database, no mocking needed
    Assert.Throws<InvalidOperationException>(() =>
        machine.AddProduction(100, DateTime.UtcNow));
}
```

**Comparison with other architectures**:
- ? Layered: Business logic mixed with data access ? need database for tests
- ? Onion: Better but still coupled to repository abstractions
- ? Clean: Domain is completely isolated ? unit tests are fast and simple

### 3. **Rich Domain Model with Value Objects**

**Example**: `MachineName` Value Object prevents primitive obsession:

```csharp
[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct MachineName
{
    public const int MaxLength = 100;
    public const int MinLength = 2;

    private static Validation Validate(in string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Validation.Invalid("Machine name cannot be empty");
        
        if (name.Length < MinLength)
            return Validation.Invalid($"Machine name must be at least {MinLength} characters");
        
        if (name.Length > MaxLength)
            return Validation.Invalid($"Machine name cannot exceed {MaxLength} characters");
        
        return Validation.Ok;
    }
}
```

**Benefits**:
- ? Validation happens at **domain level**, not in controllers or services
- ? Impossible to create invalid `MachineName` - **compile-time safety**
- ? Type-safe: can't accidentally pass `string` where `MachineName` is expected
- ? Self-documenting code: `MachineName` is more meaningful than `string`

### 4. **SmartEnum for Business Logic**

**Example**: `MachineStatus` encapsulates status-related rules:

```csharp
public class MachineStatus : SmartEnum<MachineStatus>
{
    public static readonly MachineStatus Active = new(nameof(Active), 1);
    public static readonly MachineStatus Inactive = new(nameof(Inactive), 0);
    public static readonly MachineStatus Maintenance = new(nameof(Maintenance), 2);
    public static readonly MachineStatus Decommissioned = new(nameof(Decommissioned), 3);

    // Business rules encoded in the type
    public bool CanRecordProduction => this == Active;
    public bool CanBeDeleted => this == Inactive || this == Decommissioned;
}
```

**Comparison**:
- ? Layered: Status is just an enum, rules scattered in services
- ? Onion: Similar issues with primitive types
- ? Clean: Business rules **live with the data** they govern

### 5. **Domain Events for Loose Coupling**

**Example**: `MachineCreatedEvent` enables side effects without tight coupling:

```csharp
// Domain raises event
public Machine(MachineName name, string? description = null)
{
    Name = name;
    Description = description;
    
    RegisterDomainEvent(new MachineCreatedEvent(this));
}

// Infrastructure handles it asynchronously
public class MachineCreatedNotificationHandler : INotificationHandler<MachineCreatedEvent>
{
    public async ValueTask Handle(MachineCreatedEvent notification, ...)
    {
        // Send email, update dashboard, log analytics
        await _emailSender.SendEmailAsync(...);
    }
}
```

**Advantages**:
- ? **Machine entity** doesn't know about email sending
- ? Add/remove event handlers without modifying domain
- ? Multiple handlers can react to the same event
- ? Domain remains **pure** and **focused**

### 6. **CQRS Pattern in Use Cases**

**Commands** (change state):

```csharp
public record AddProductionCommand(
    int MachineId,
    int Quantity,
    DateTime ProducedAt,
    string? BatchNumber) : ICommand<Result>;

public class AddProductionHandler : ICommandHandler<AddProductionCommand, Result>
{
    public async ValueTask<Result> Handle(...)
    {
        // Fetch aggregate
        var machine = await _repository.FirstOrDefaultAsync(spec, ...);
        
        // Execute domain logic
        machine.AddProduction(command.Quantity, command.ProducedAt, command.BatchNumber);
        
        // Persist
        await _repository.UpdateAsync(machine, ...);
        
        return Result.Success();
    }
}
```

**Queries** (read data) - bypass domain for performance:

```csharp
public class ListMachinesQueryService : IListMachinesQueryService
{
    public async Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(...)
    {
        // Direct database query - optimized for reads
        return await _dbContext.Machines
            .AsNoTracking()
            .Select(m => new MachineDTO(...))
            .ToListAsync(...);
    }
}
```

**Benefits**:
- ? **Writes** go through domain ? business rules enforced
- ? **Reads** optimized with projections ? better performance
- ? Separation of concerns ? easier to optimize each independently

### 7. **Specification Pattern**

**Example**: Encapsulate query logic in the domain:

```csharp
public sealed class MachineByIdSpec : Specification<Machine>
{
    public MachineByIdSpec(MachineId machineId)
    {
        Query.Where(m => m.Id == machineId);
    }
}

// Usage in handler
var spec = new MachineByIdSpec(machineId);
var machine = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
```

**Why specifications**:
- ? Query logic **belongs to the domain**, not infrastructure
- ? Reusable across use cases
- ? Testable independently
- ? Can be combined for complex queries

### 8. **Guard Clauses for Validation**

**Example**: Fail fast with clear error messages:

```csharp
public sealed record MachineProduction
{
    public MachineProduction(int quantity, DateTime producedAt, DateTime recordedAt, ...)
    {
        Guard.Against.NegativeOrZero(quantity, nameof(quantity), 
            "Production quantity must be positive");
        Guard.Against.Default(producedAt, nameof(producedAt));
        
        if (producedAt > recordedAt)
            throw new ArgumentException(
                "Production date cannot be in the future relative to recording date");
        
        Quantity = quantity;
        ProducedAt = producedAt;
        RecordedAt = recordedAt;
    }
}
```

**Advantages**:
- ? **Explicit** validation at construction
- ? Impossible to create invalid objects
- ? Clear error messages for debugging

### 9. **Business Rules in Domain, Not Controllers**

**Comparison**: Deleting a machine

**? Layered Architecture** (rules in service layer):
```csharp
public async Task<bool> DeleteMachine(int id)
{
    var machine = await _repo.GetById(id);
    
    // Business rules in service - scattered logic
    if (machine.Status == "Active") return false;
    if (machine.Productions.Any(p => p.Date > DateTime.Now.AddDays(-30))) 
        return false;
    
    await _repo.Delete(id);
    return true;
}
```

**? Clean Architecture** (rules in domain):
```csharp
// Domain Entity
public void EnsureCanBeDeleted()
{
    if (!Status.CanBeDeleted)
        throw new InvalidOperationException(
            $"Machine in {Status.Name} status cannot be deleted");
    
    if (HasRecentProduction())
        throw new InvalidOperationException(
            "Cannot delete machine with production records in the last 30 days");
}

// Use Case Handler - just orchestrates
public async ValueTask<Result> Handle(DeleteMachineCommand command, ...)
{
    var machine = await _repository.FirstOrDefaultAsync(spec, ...);
    
    machine.EnsureCanBeDeleted(); // Domain enforces rules
    
    await _repository.DeleteAsync(machine, ...);
    return Result.Success();
}
```

**Result**: Business rules are **centralized, testable, and enforced everywhere**.

### 10. **Dependency Inversion**

**Example**: Core defines interfaces, Infrastructure implements them:

```csharp
// Core layer - defines what it needs
public interface IListMachinesQueryService
{
    Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(...);
}

// Infrastructure layer - provides implementation
public class ListMachinesQueryService : IListMachinesQueryService
{
    public async Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(...)
    {
        // EF Core implementation
        return await _dbContext.Machines.AsNoTracking()...;
    }
}
```

**Power of this approach**:
- ? Core doesn't know about EF Core, SQL Server, etc.
- ? Can swap implementations without changing use cases
- ? Can have multiple implementations (in-memory for tests, SQL for production)

---

## ?? Use Case: Adding Production to a Machine

Let's trace a request through all layers:

### 1. **Web Layer** (Presentation)

```csharp
public class AddProduction : Endpoint<AddProductionRequest, ...>
{
    public override async Task ExecuteAsync(AddProductionRequest request, ...)
    {
        var command = new AddProductionCommand(
            request.MachineId,
            request.Quantity,
            request.ProducedAt,
            request.BatchNumber);
        
        var result = await _mediator.Send(command);
        // HTTP concerns only - no business logic here
    }
}
```

### 2. **Use Cases Layer** (Application Logic)

```csharp
public class AddProductionHandler : ICommandHandler<AddProductionCommand, Result>
{
    public async ValueTask<Result> Handle(...)
    {
        // 1. Fetch aggregate
        var machine = await _repository.FirstOrDefaultAsync(spec, ...);
        
        // 2. Execute domain logic (business rules enforced here)
        machine.AddProduction(command.Quantity, command.ProducedAt, command.BatchNumber);
        
        // 3. Persist
        await _repository.UpdateAsync(machine, ...);
        
        return Result.Success();
    }
}
```

### 3. **Core Layer** (Domain Logic)

```csharp
public Machine AddProduction(int quantity, DateTime producedAt, string? batchNumber)
{
    // Business rule: Only active machines can record production
    if (!Status.CanRecordProduction)
        throw new InvalidOperationException(...);
    
    var production = new MachineProduction(quantity, producedAt, DateTime.UtcNow, batchNumber);
    _productions.Add(production);
    LastProductionAt = producedAt;
    
    // Raise domain event
    RegisterDomainEvent(new MachineProductionAddedEvent(this, production));
    
    return this;
}
```

### 4. **Infrastructure Layer** (External Concerns)

```csharp
// EF Core handles persistence
public class AppDbContext : DbContext
{
    public DbSet<Machine> Machines => Set<Machine>();
}

// Event handler sends notifications
public class MachineProductionAddedHandler : INotificationHandler<...>
{
    public ValueTask Handle(MachineProductionAddedEvent notification, ...)
    {
        // Log, send email, update dashboard
        _logger.LogInformation("Production recorded: {Quantity} units", ...);
        return ValueTask.CompletedTask;
    }
}
```

**Key observations**:
- Business rule ("only active machines") is in **Core**, not scattered
- Web layer knows **nothing** about business rules
- Use Cases orchestrate without duplicating domain logic
- Infrastructure is a **plugin** that can be replaced

---

## ?? Comparison with Other Architectures

| Aspect | Layered | Onion | Hexagonal | **Clean** |
|--------|---------|-------|-----------|-----------|
| **Domain Independence** | ? Coupled to data layer | ?? Better but coupled to interfaces | ? Good | ? **Excellent** |
| **Testability** | ? Needs database | ?? Needs mocking | ? Good | ? **Best** - pure domain |
| **Business Rules** | ? Scattered in services | ?? In domain but mixed with persistence concerns | ? In domain | ? **Pure domain** |
| **Value Objects** | ? Primitive obsession | ?? Not emphasized | ?? Optional | ? **Core pattern** |
| **Domain Events** | ? Not a pattern | ? Not a pattern | ?? Optional | ? **First-class** |
| **CQRS Support** | ? Not inherent | ? Not inherent | ?? Can add | ? **Built-in** |
| **Framework Independence** | ? Tightly coupled | ?? Better | ? Good | ? **Best** |
| **Learning Curve** | ? Easy | ?? Moderate | ?? Moderate | ?? **Steep but worth it** |

---

## ?? When to Use Clean Architecture

### ? **Use Clean Architecture when**:
1. **Complex Business Logic**: Your application has sophisticated business rules
2. **Long-term Maintenance**: Project will be maintained for years
3. **Multiple Interfaces**: Need to support Web API, gRPC, CLI, etc.
4. **Team Size**: Medium to large teams benefit from clear boundaries
5. **Testability Critical**: High test coverage required
6. **Framework Flexibility**: May need to change frameworks/databases

### ? **Don't use Clean Architecture for**:
1. **Simple CRUD**: Basic Create-Read-Update-Delete operations
2. **Prototypes**: Rapid prototyping where architecture overhead slows you down
3. **Small Scripts**: Utility scripts or one-off tools
4. **Tight Deadlines**: When time-to-market is more important than maintainability

---

## ?? Key Takeaways

1. **Dependency Rule**: Dependencies point inward. Core has zero dependencies.

2. **Business Logic in Core**: All business rules live in the domain, not services or controllers.

3. **Value Objects**: Strongly-typed domain concepts prevent primitive obsession.

4. **Domain Events**: Enable loose coupling and side effects without domain contamination.

5. **CQRS**: Separate reads (optimized queries) from writes (domain model).

6. **Testability**: Pure domain logic can be tested without infrastructure.

7. **Framework Independence**: Core doesn't depend on ASP.NET, EF Core, or any framework.

8. **Flexibility**: Easy to swap databases, add new interfaces, or change frameworks.

---

## ?? Clean Architecture in .NET 10

This implementation leverages **modern C# features**:

- **Primary constructors** (C# 12): Concise dependency injection
- **Record types**: Immutable DTOs and value objects
- **ValueTask**: Efficient async operations
- **Collection expressions `[]`**: Modern collection initialization
- **Vogen**: Code generation for value objects
- **SmartEnum**: Rich enums with behavior
- **Mediator pattern**: Decoupled message handling

---

## Conclusion

Clean Architecture provides **maximum flexibility, testability, and maintainability** at the cost of initial complexity. For machine monitoring systems with complex business rules, regulatory requirements, and long-term maintenance needs, this investment pays significant dividends.

The separation of concerns ensures that:
- ? Business rules are **explicit and enforceable**
- ? Tests are **fast and comprehensive**
- ? Changes are **isolated and safe**
- ? Code is **self-documenting**
- ? Architecture is **future-proof**

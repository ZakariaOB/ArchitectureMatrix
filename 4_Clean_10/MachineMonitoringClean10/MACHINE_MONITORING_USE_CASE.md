# Machine Monitoring Use Case - Clean Architecture Strengths

## Overview

This document demonstrates how the **Machine Monitoring** use case in the Clean_10 project showcases the **key strengths of Clean Architecture** through concrete implementation examples.

---

## ?? Use Case: Machine Production Recording

### Business Scenario

A manufacturing facility needs to track production output for each machine. The system must:

1. ? **Only allow active machines to record production**
2. ? **Validate production data** (positive quantities, valid dates)
3. ? **Maintain production history** for reporting and analysis
4. ? **Notify operators** when production is recorded
5. ? **Update dashboards** in real-time

---

## ?? Clean Architecture Solution

### Implementation Across Layers

```
???????????????????????????????????????????????????????????
?              WEB LAYER (Presentation)                    ?
?  POST /Machines/{id}/Production                          ?
?  ? Validates HTTP request                                ?
?  ? Maps to Command                                       ?
???????????????????????????????????????????????????????????
                           ?
???????????????????????????????????????????????????????????
?           USE CASES LAYER (Application Logic)            ?
?  AddProductionHandler                                    ?
?  ? Orchestrates workflow                                 ?
?  ? No business rules here!                               ?
???????????????????????????????????????????????????????????
                           ?
???????????????????????????????????????????????????????????
?              CORE LAYER (Domain Logic)                   ?
?  Machine.AddProduction()                                 ?
?  ? Enforces: "Only active machines can record"           ?
?  ? Validates production data                             ?
?  ? Raises domain event                                   ?
???????????????????????????????????????????????????????????
                           ?
???????????????????????????????????????????????????????????
?        INFRASTRUCTURE LAYER (External Concerns)          ?
?  ? Persists to database (EF Core)                        ?
?  ? Sends notifications (Event Handler)                   ?
?  ? Updates dashboards (Event Handler)                    ?
???????????????????????????????????????????????????????????
```

---

## ?? Strength #1: Business Rules in Domain

### ? **Traditional Layered Architecture** (Anti-Pattern)

```csharp
// Controller has business logic - WRONG!
[HttpPost("machines/{id}/production")]
public async Task<IActionResult> AddProduction(int id, ProductionRequest request)
{
    var machine = await _db.Machines.FindAsync(id);
    
    // Business rule scattered in controller
    if (machine.Status != "Active")
        return BadRequest("Machine must be active");
    
    // More business logic in controller
    if (request.Quantity <= 0)
        return BadRequest("Quantity must be positive");
    
    // Direct database manipulation
    _db.Productions.Add(new Production 
    { 
        MachineId = id, 
        Quantity = request.Quantity 
    });
    
    await _db.SaveChangesAsync();
    return Ok();
}
```

**Problems**:
- ? Business rules duplicated across endpoints
- ? Rules can be bypassed by calling database directly
- ? Impossible to unit test without HTTP context
- ? No protection from invalid states

---

### ? **Clean Architecture** (Correct Pattern)

#### **1. Web Layer** - HTTP Concerns Only

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Web/Machines/AddProduction.cs`

```csharp
/// <summary>
/// Web endpoint - handles HTTP concerns ONLY
/// No business logic here!
/// </summary>
public class AddProduction(IMediator mediator)
  : Endpoint<AddProductionRequest, Results<Ok, NotFound, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post("/Machines/{MachineId:int}/Production");
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Record production for a machine";
      s.Description = "Machine must be in Active status.";
    });
    Tags("Machines");
  }

  public override async Task<Results<Ok, NotFound, ProblemHttpResult>>
    ExecuteAsync(AddProductionRequest request, CancellationToken cancellationToken)
  {
    // Only maps HTTP request to Command - no business logic
    var command = new AddProductionCommand(
      request.MachineId,
      request.Quantity,
      request.ProducedAt,
      request.BatchNumber);

    var result = await mediator.Send(command);

    // Maps result to HTTP response - no business logic
    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(),
      ResultStatus.NotFound => TypedResults.NotFound(),
      _ => TypedResults.Problem(...)
    };
  }
}

public class AddProductionRequest
{
  public const string Route = "/Machines/{MachineId:int}/Production";
  
  public int MachineId { get; set; }
  
  [Required]
  public int Quantity { get; set; }
  
  [Required]
  public DateTime ProducedAt { get; set; }
  
  public string? BatchNumber { get; set; }
}

// Validation - NOT business rules, just HTTP input validation
public class AddProductionValidator : Validator<AddProductionRequest>
{
  public AddProductionValidator()
  {
    RuleFor(x => x.Quantity)
      .GreaterThan(0)
      .WithMessage("Quantity must be positive.");
    
    RuleFor(x => x.ProducedAt)
      .NotEmpty()
      .LessThanOrEqualTo(DateTime.UtcNow)
      .WithMessage("Production date cannot be in the future.");
  }
}
```

**? Strengths Demonstrated**:
- Web layer is **thin** - only HTTP concerns
- Easy to **change web framework** without touching business rules
- Can add **gRPC, CLI, or SignalR** interfaces without code duplication

---

#### **2. Use Cases Layer** - Application Orchestration

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.UseCases/Machines/AddProduction/AddProductionHandler.cs`

```csharp
/// <summary>
/// Use Case Handler - orchestrates the workflow
/// Business rules are in the DOMAIN, not here!
/// </summary>
public class AddProductionHandler(IRepository<Machine> repository)
  : ICommandHandler<AddProductionCommand, Result>
{
  public async ValueTask<Result> Handle(
    AddProductionCommand command,
    CancellationToken cancellationToken)
  {
    // 1. Fetch the aggregate
    var machineId = MachineId.From(command.MachineId);
    var spec = new MachineByIdSpec(machineId);
    var machine = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (machine is null)
    {
      return Result.NotFound("Machine not found");
    }

    // 2. Execute domain logic
    // ? This is where the MAGIC happens!
    // Business rule "only active machines can record production" 
    // is enforced INSIDE the domain entity
    machine.AddProduction(command.Quantity, command.ProducedAt, command.BatchNumber);

    // 3. Persist changes
    await repository.UpdateAsync(machine, cancellationToken);

    return Result.Success();
  }
}

/// <summary>
/// Command - represents the intent to change state
/// CQRS pattern: Commands modify, Queries read
/// </summary>
public record AddProductionCommand(
  int MachineId,
  int Quantity,
  DateTime ProducedAt,
  string? BatchNumber) : ICommand<Result>;
```

**? Strengths Demonstrated**:
- Use Case **orchestrates** without duplicating business logic
- **Testable** independently from web layer and infrastructure
- **Single Responsibility** - just coordinates between layers
- **CQRS** - clear separation between commands and queries

---

#### **3. Core Layer** - Business Rules (The Heart!)

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Core/MachineAggregate/Machine.cs`

```csharp
/// <summary>
/// Machine Aggregate Root - ALL business rules live here!
/// This is the HEART of Clean Architecture.
/// </summary>
public class Machine : EntityBase<Machine, MachineId>, IAggregateRoot
{
  public MachineName Name { get; private set; }
  public MachineStatus Status { get; private set; } = MachineStatus.Inactive;
  public DateTime? LastProductionAt { get; private set; }

  private readonly List<MachineProduction> _productions = [];
  public IReadOnlyCollection<MachineProduction> Productions => _productions.AsReadOnly();

  /// <summary>
  /// Records production for this machine.
  /// 
  /// ? BUSINESS RULE ENFORCEMENT:
  /// - Only active machines can record production
  /// - Production data must be valid
  /// - Domain event is raised for side effects
  /// 
  /// This method CANNOT be bypassed - it's the ONLY way to add production.
  /// </summary>
  public Machine AddProduction(int quantity, DateTime producedAt, string? batchNumber = null)
  {
    // ? BUSINESS RULE #1: Only active machines can record production
    if (!Status.CanRecordProduction)
      throw new InvalidOperationException(
        $"Cannot record production for a machine in {Status.Name} status. " +
        $"Machine must be Active.");

    // ? Create value object with validation
    // This throws if quantity is invalid - "Parse, Don't Validate" principle
    var production = new MachineProduction(quantity, producedAt, DateTime.UtcNow, batchNumber);
    
    // ? Add to internal collection (encapsulated)
    _productions.Add(production);
    LastProductionAt = producedAt;

    // ? Raise domain event for side effects (notifications, dashboards)
    RegisterDomainEvent(new MachineProductionAddedEvent(this, production));

    return this;
  }

  /// <summary>
  /// Gets total production quantity for this machine.
  /// Domain logic stays with the data it operates on.
  /// </summary>
  public int GetTotalProduction() => _productions.Sum(p => p.Quantity);

  /// <summary>
  /// Gets production within a date range.
  /// </summary>
  public int GetProductionBetween(DateTime start, DateTime end) =>
    _productions
      .Where(p => p.ProducedAt >= start && p.ProducedAt <= end)
      .Sum(p => p.Quantity);
}
```

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Core/MachineAggregate/MachineStatus.cs`

```csharp
/// <summary>
/// SmartEnum - encapsulates status-related business rules
/// ? Business rules live with the data they govern
/// </summary>
public class MachineStatus : SmartEnum<MachineStatus>
{
  public static readonly MachineStatus Inactive = new(nameof(Inactive), 0);
  public static readonly MachineStatus Active = new(nameof(Active), 1);
  public static readonly MachineStatus Maintenance = new(nameof(Maintenance), 2);
  public static readonly MachineStatus Decommissioned = new(nameof(Decommissioned), 3);

  private MachineStatus(string name, int value) : base(name, value) { }

  /// <summary>
  /// ? BUSINESS RULE: Only active machines can record production
  /// This rule is checked by Machine.AddProduction()
  /// </summary>
  public bool CanRecordProduction => this == Active;

  /// <summary>
  /// ? BUSINESS RULE: Only inactive/decommissioned machines can be deleted
  /// </summary>
  public bool CanBeDeleted => this == Inactive || this == Decommissioned;
}
```

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Core/MachineAggregate/MachineProduction.cs`

```csharp
/// <summary>
/// Value Object - immutable and self-validating
/// ? "Parse, Don't Validate" principle
/// </summary>
public sealed record MachineProduction
{
  public int Quantity { get; }
  public DateTime ProducedAt { get; }
  public DateTime RecordedAt { get; }
  public string? BatchNumber { get; }

  public MachineProduction(int quantity, DateTime producedAt, DateTime recordedAt, string? batchNumber = null)
  {
    // ? Guard clauses - fail fast with clear messages
    Guard.Against.NegativeOrZero(quantity, nameof(quantity), 
      "Production quantity must be positive");
    Guard.Against.Default(producedAt, nameof(producedAt));
    Guard.Against.Default(recordedAt, nameof(recordedAt));

    // ? Domain rule: production date can't be in the future
    if (producedAt > recordedAt)
      throw new ArgumentException(
        "Production date cannot be in the future relative to recording date");

    Quantity = quantity;
    ProducedAt = producedAt;
    RecordedAt = recordedAt;
    BatchNumber = batchNumber;
  }
}
```

**? Strengths Demonstrated**:
- ? **Business rules centralized** in one place
- ? **Impossible to bypass** - domain enforces invariants
- ? **Self-documenting** - code explains business rules
- ? **Testable** without any infrastructure
- ? **SmartEnum** encapsulates status logic
- ? **Value Objects** prevent invalid states
- ? **Domain Events** for loose coupling

---

#### **4. Infrastructure Layer** - External Concerns

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Core/MachineAggregate/Handlers/MachineProductionAddedHandler.cs`

```csharp
/// <summary>
/// Domain Event Handler - reacts to production being recorded
/// ? Side effects are separated from core business logic
/// </summary>
public class MachineProductionAddedHandler(ILogger<MachineProductionAddedHandler> logger) 
  : INotificationHandler<MachineProductionAddedEvent>
{
  public ValueTask Handle(MachineProductionAddedEvent notification, CancellationToken cancellationToken)
  {
    // Log for auditing
    logger.LogInformation(
      "Production recorded for machine {MachineName}: {Quantity} units (Batch: {BatchNumber})",
      notification.Machine.Name,
      notification.Production.Quantity,
      notification.Production.BatchNumber ?? "N/A");

    // In a real application:
    // - Update real-time dashboards (SignalR)
    // - Check against production targets
    // - Trigger alerts for anomalies
    // - Send notifications to supervisors
    // - Update analytics systems

    return ValueTask.CompletedTask;
  }
}
```

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Infrastructure/Data/Config/MachineConfiguration.cs`

```csharp
/// <summary>
/// EF Core configuration - completely separate from domain
/// ? Domain doesn't know about database details
/// </summary>
public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
  public void Configure(EntityTypeBuilder<Machine> builder)
  {
    builder.ToTable("Machines");

    // Configure value objects with Vogen converters
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, Machine, MachineId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Name)
      .HasVogenConversion()
      .HasMaxLength(MachineName.MaxLength)
      .IsRequired();

    // Configure SmartEnum
    builder.Property(x => x.Status)
      .HasConversion(
          x => x.Value,
          x => MachineStatus.FromValue(x))
      .IsRequired();

    // Configure owned collection (aggregate pattern)
    builder.OwnsMany(entity => entity.Productions, productionBuilder =>
    {
      productionBuilder.ToTable("MachineProductions");
      productionBuilder.Property(p => p.Quantity).IsRequired();
      productionBuilder.Property(p => p.ProducedAt).IsRequired();
      productionBuilder.Property(p => p.RecordedAt).IsRequired();
      productionBuilder.Property(p => p.BatchNumber).HasMaxLength(50);
    });
  }
}
```

**? Strengths Demonstrated**:
- ? **Domain Events** enable loose coupling
- ? **Infrastructure** is a plugin that can be replaced
- ? **ORM mapping** separate from domain logic
- ? **Can switch databases** without touching domain

---

## ?? Strength #2: Testability at Every Layer

### Unit Test - Pure Domain Logic (No Dependencies!)

```csharp
public class MachineProductionTests
{
  [Fact]
  public void AddProduction_WhenMachineActive_AddsProductionSuccessfully()
  {
    // Arrange - Pure domain objects, no mocking!
    var machine = new Machine(MachineName.From("CNC-001"));
    machine.Activate(); // Set to Active status

    // Act
    machine.AddProduction(100, DateTime.UtcNow, "BATCH-001");

    // Assert
    Assert.Equal(100, machine.GetTotalProduction());
    Assert.NotNull(machine.LastProductionAt);
    Assert.Single(machine.Productions);
  }

  [Fact]
  public void AddProduction_WhenMachineInactive_ThrowsException()
  {
    // Arrange
    var machine = new Machine(MachineName.From("CNC-001"));
    // Machine is Inactive by default

    // Act & Assert - Clear error message
    var exception = Assert.Throws<InvalidOperationException>(() =>
      machine.AddProduction(100, DateTime.UtcNow));
    
    Assert.Contains("Cannot record production", exception.Message);
    Assert.Contains("Inactive", exception.Message);
  }

  [Fact]
  public void AddProduction_WithNegativeQuantity_ThrowsException()
  {
    // Arrange
    var machine = new Machine(MachineName.From("CNC-001"));
    machine.Activate();

    // Act & Assert - Value Object validation
    var exception = Assert.Throws<ArgumentException>(() =>
      machine.AddProduction(-10, DateTime.UtcNow));
    
    Assert.Contains("Production quantity must be positive", exception.Message);
  }

  [Fact]
  public void AddProduction_RaisesDomainEvent()
  {
    // Arrange
    var machine = new Machine(MachineName.From("CNC-001"));
    machine.Activate();

    // Act
    machine.AddProduction(100, DateTime.UtcNow);

    // Assert - Domain event was raised
    var domainEvents = machine.DomainEvents;
    Assert.Single(domainEvents);
    Assert.IsType<MachineProductionAddedEvent>(domainEvents.First());
  }
}
```

**? Benefits**:
- ? **No mocking** - pure domain logic
- ? **Fast** - no database, no HTTP
- ? **Clear** - tests document business rules
- ? **Reliable** - no external dependencies can fail

---

### Integration Test - Use Case Handler

```csharp
public class AddProductionHandlerTests
{
  [Fact]
  public async Task Handle_WhenMachineNotFound_ReturnsNotFound()
  {
    // Arrange
    var mockRepository = new Mock<IRepository<Machine>>();
    mockRepository
      .Setup(r => r.FirstOrDefaultAsync(It.IsAny<MachineByIdSpec>(), default))
      .ReturnsAsync((Machine?)null);

    var handler = new AddProductionHandler(mockRepository.Object);
    var command = new AddProductionCommand(999, 100, DateTime.UtcNow, null);

    // Act
    var result = await handler.Handle(command, default);

    // Assert
    Assert.Equal(ResultStatus.NotFound, result.Status);
  }

  [Fact]
  public async Task Handle_WhenMachineInactive_ThrowsException()
  {
    // Arrange
    var inactiveMachine = new Machine(MachineName.From("CNC-001"));
    // Machine is Inactive by default

    var mockRepository = new Mock<IRepository<Machine>>();
    mockRepository
      .Setup(r => r.FirstOrDefaultAsync(It.IsAny<MachineByIdSpec>(), default))
      .ReturnsAsync(inactiveMachine);

    var handler = new AddProductionHandler(mockRepository.Object);
    var command = new AddProductionCommand(1, 100, DateTime.UtcNow, null);

    // Act & Assert - Domain throws exception
    await Assert.ThrowsAsync<InvalidOperationException>(async () =>
      await handler.Handle(command, default));
  }
}
```

**? Benefits**:
- ? **Minimal mocking** - only repository
- ? **Tests orchestration** - not business rules
- ? **Fast** - no database needed

---

## ?? Strength #3: Multiple Interfaces Without Code Duplication

The business rule **"only active machines can record production"** is enforced in the domain. This means we can add ANY interface without duplicating this logic:

### REST API (Current Implementation)

```csharp
// FastEndpoints
POST /Machines/{id}/Production
```

### gRPC Service (Easy to Add)

```csharp
public class MachineService : Machines.MachinesBase
{
  public override async Task<AddProductionResponse> AddProduction(
    AddProductionRequest request, ServerCallContext context)
  {
    var command = new AddProductionCommand(
      request.MachineId, request.Quantity, ...);
    
    var result = await _mediator.Send(command);
    // Same business rules enforced!
    return MapToGrpcResponse(result);
  }
}
```

### CLI Tool (Easy to Add)

```csharp
[Command("add-production")]
public class AddProductionCommand : AsyncCommand<AddProductionSettings>
{
  public override async Task<int> ExecuteAsync(
    CommandContext context, AddProductionSettings settings)
  {
    var command = new AddProductionCommand(
      settings.MachineId, settings.Quantity, ...);
    
    var result = await _mediator.Send(command);
    // Same business rules enforced!
    return result.IsSuccess ? 0 : 1;
  }
}
```

### SignalR Hub (Real-time Updates)

```csharp
public class MachineHub : Hub
{
  public async Task AddProduction(int machineId, int quantity, DateTime producedAt)
  {
    var command = new AddProductionCommand(machineId, quantity, producedAt, null);
    var result = await _mediator.Send(command);
    // Same business rules enforced!
    
    await Clients.All.SendAsync("ProductionRecorded", machineId, quantity);
  }
}
```

**? Key Point**: Business rule is enforced **once** in the domain, but works across **all interfaces**!

---

## ?? Strength #4: CQRS - Optimized Reads and Writes

### Commands (Write) - Go Through Domain

```csharp
// ? Writes MUST go through the domain to enforce business rules
public class AddProductionHandler : ICommandHandler<AddProductionCommand, Result>
{
  public async ValueTask<Result> Handle(...)
  {
    var machine = await _repository.FirstOrDefaultAsync(...); // Load aggregate
    machine.AddProduction(...); // Business rules enforced
    await _repository.UpdateAsync(machine, ...); // Persist changes
    return Result.Success();
  }
}
```

### Queries (Read) - Bypass Domain for Performance

**File**: `4_Clean_10/MachineMonitoringClean10/src/MachineMonitoringClean10.Infrastructure/Data/Queries/ListMachinesQueryService.cs`

```csharp
/// <summary>
/// ? Read queries can bypass the domain model for performance
/// No business rules to enforce on reads!
/// </summary>
public class ListMachinesQueryService(AppDbContext dbContext) : IListMachinesQueryService
{
  public async Task<(IReadOnlyList<MachineDTO> Items, int TotalCount)> ListAsync(
    int? skip, int? take, CancellationToken cancellationToken)
  {
    var query = dbContext.Machines.AsNoTracking(); // Direct EF query

    var totalCount = await query.CountAsync(cancellationToken);

    var machines = await query
      .OrderBy(m => m.Name)
      .Skip(skip ?? 0)
      .Take(take ?? 10)
      .Select(m => new MachineDTO( // Project directly to DTO
        m.Id.Value,
        m.Name.Value,
        m.Description,
        m.Status.Name,
        m.CreatedAt,
        m.LastProductionAt,
        m.Productions.Sum(p => p.Quantity))) // Calculated in query
      .ToListAsync(cancellationToken);

    return (machines, totalCount);
  }
}
```

**? Benefits**:
- ? **Writes** - Business rules always enforced
- ? **Reads** - Optimized with SQL projections
- ? **Performance** - Best of both worlds
- ? **Correctness** - Rules can't be bypassed

---

## ?? Comparison: Clean vs. Traditional Layered

| Aspect | Layered Architecture | **Clean Architecture** |
|--------|---------------------|------------------------|
| **Business Rules Location** | Scattered in controllers, services | **? Centralized in domain** |
| **Rule Enforcement** | Can be bypassed | **? Impossible to bypass** |
| **Testing** | Needs database/HTTP | **? Pure unit tests** |
| **Multiple Interfaces** | Duplicate code | **? Reuse domain logic** |
| **CQRS** | Manual implementation | **? Natural pattern** |
| **Value Objects** | Primitives everywhere | **? Type-safe domain concepts** |
| **Domain Events** | Tightly coupled | **? Loosely coupled** |
| **Framework Changes** | Major refactoring | **? Swap outer layers only** |
| **Code Clarity** | Business rules hidden | **? Self-documenting** |

---

## ?? Real-World Scenarios

### Scenario 1: Adding a Mobile App

**Traditional Layered**:
- ? Must duplicate business logic in mobile backend
- ? Rules can drift between web and mobile
- ? Two codebases to maintain

**Clean Architecture**:
- ? Mobile app calls same Use Cases via new API endpoints
- ? Business rules enforced consistently
- ? Single source of truth

### Scenario 2: Switching from SQL Server to PostgreSQL

**Traditional Layered**:
- ? Business logic mixed with data access
- ? Risk of breaking business rules during migration

**Clean Architecture**:
- ? Only Infrastructure layer changes
- ? Domain and Use Cases unchanged
- ? Business rules remain intact

### Scenario 3: Regulatory Audit

**Traditional Layered**:
- ? Business rules scattered across codebase
- ? Hard to prove compliance

**Clean Architecture**:
- ? All business rules in one place (Core)
- ? Easy to demonstrate compliance
- ? Self-documenting code

---

## ?? Key Takeaways

### 1. **Business Rules in Domain**
```csharp
// ? The rule "only active machines" lives in Machine.AddProduction()
// It's enforced EVERYWHERE - web, API, CLI, tests
if (!Status.CanRecordProduction)
  throw new InvalidOperationException(...);
```

### 2. **Use Cases Orchestrate, Don't Duplicate**
```csharp
// ? Handler just coordinates - no business logic here
var machine = await _repository.FirstOrDefaultAsync(...);
machine.AddProduction(...); // Domain does the work
await _repository.UpdateAsync(...);
```

### 3. **Value Objects Prevent Invalid States**
```csharp
// ? Impossible to create invalid MachineProduction
// Validation happens at construction
var production = new MachineProduction(quantity, producedAt, recordedAt);
```

### 4. **Domain Events Enable Loose Coupling**
```csharp
// ? Domain raises event, infrastructure handles it
RegisterDomainEvent(new MachineProductionAddedEvent(this, production));
```

### 5. **CQRS Optimizes Performance**
```csharp
// ? Writes through domain, reads bypass for performance
// Command: machine.AddProduction(...) 
// Query: dbContext.Machines.Select(m => new DTO(...))
```

---

## ?? Conclusion

The **Machine Production Recording** use case demonstrates that Clean Architecture:

? **Protects business rules** - impossible to bypass  
? **Enables testing** - pure domain logic  
? **Supports multiple interfaces** - no code duplication  
? **Facilitates change** - swap outer layers easily  
? **Documents itself** - business rules are explicit  
? **Scales well** - clear boundaries prevent coupling  

The investment in structure pays dividends through **maintainability, testability, and flexibility**.

---

## ?? Further Reading

- **Full Architecture Guide**: See `CLEAN_ARCHITECTURE_GUIDE.md` for complete patterns
- **Core Domain**: Explore `src/MachineMonitoringClean10.Core/MachineAggregate/`
- **Use Cases**: See `src/MachineMonitoringClean10.UseCases/Machines/`
- **API Endpoints**: Check `src/MachineMonitoringClean10.Web/Machines/`

---

**Question**: *Can you find where the business rule "only active machines can record production" is enforced in a traditional layered architecture? In Clean Architecture, it's in one place: `Machine.AddProduction()`. That's the power of Clean Architecture.*

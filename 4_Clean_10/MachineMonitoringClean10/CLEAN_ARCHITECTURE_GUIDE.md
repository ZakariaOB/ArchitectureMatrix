# Clean Architecture in Machine Monitoring - .NET 10 Implementation

## Overview

This document explains the Machine Monitoring implementation in the **Clean Architecture** pattern using .NET 10, 
highlighting the **strengths and advantages** of this architectural approach compared to Layered, Onion, and Hexagonal architectures.

---

## 📐 Clean Architecture Structure

```
┌─────────────────────────────────────────────────────────┐
│                      Presentation                        │
│              (Web API - FastEndpoints)                   │
│  Controllers, Validators, HTTP concerns                  │
└───────────────────────┬─────────────────────────────────┘
                        │ Depends on
┌───────────────────────▼─────────────────────────────────┐
│                     Use Cases                            │
│         (Application Business Rules)                     │
│  Commands, Queries, Handlers, DTOs                       │
└───────────────────────┬─────────────────────────────────┘
                        │ Depends on
┌───────────────────────▼─────────────────────────────────┐
│                       Core                               │
│            (Enterprise Business Rules)                   │
│  Entities, Value Objects, Domain Events                  │
│  Aggregates, Specifications, Interfaces                  │
└─────────────────────────────────────────────────────────┘
                        ▲
                        │ Implements
┌───────────────────────┴─────────────────────────────────┐
│                 Infrastructure                           │
│       (External Concerns - Plugins)                      │
│  Database, External APIs, Email, File System             │
└─────────────────────────────────────────────────────────┘
```

**Key Principle**: Dependencies point **inward**. Core has **zero dependencies** on outer layers.

---

## 🔍 Clean vs. Hexagonal vs. Onion: What's the Difference?

All three architectures (Clean, Hexagonal, and Onion) share the same goal: **protecting the domain from external concerns**. However, they differ in their approach, emphasis, and practical implementation.

### The Common Problem They Solve

Traditional layered architectures create tight coupling between business logic and infrastructure:
- Database changes require business logic changes
- Testing requires spinning up databases
- Switching frameworks means rewriting code
- Business rules get scattered across layers

**All three architectures solve this by inverting dependencies** - making infrastructure depend on the domain, not vice versa.

---

### 🧅 Onion Architecture

**Creator**: Jeffrey Palermo (2008)

**Visual Structure**:
```
┌─────────────────────────────────────┐
│     Infrastructure (Outer Ring)     │
│  ┌───────────────────────────────┐  │
│  │   Application (Middle Ring)   │  │
│  │  ┌─────────────────────────┐  │  │
│  │  │  Domain (Core/Center)   │  │  │
│  │  │  Entities, Services     │  │  │
│  │  └─────────────────────────┘  │  │
│  └───────────────────────────────┘  │
└─────────────────────────────────────┘
```

**Key Characteristics**:
- Domain is at the **center** (like an onion)
- Dependencies point **inward** toward the core
- Outer layers can depend on inner layers
- Inner layers **never** depend on outer layers

**In Our Codebase Example** (see `1_Onion` folder):
```csharp
// Onion: Domain layer
public class Machine
{
    public int Id { get; private set; }
    public string Name { get; set; }
    private readonly List<MachineProduction> _productions = new();
    
    public void AddProduction(int totalProduction, DateTime createdDate)
    {
        if (totalProduction < 0)
            throw new ArgumentException("Production must be positive");
        
        _productions.Add(new MachineProduction(this, totalProduction, createdDate, ...));
    }
}
```

**Strengths**:
- ✅ Clear separation of concerns
- ✅ Domain is protected from external changes
- ✅ Good for Domain-Driven Design (DDD)

**Limitations**:
- ⚠️ **No explicit Use Cases layer** - application logic mixes with domain
- ⚠️ **Less focus on CQRS** - reads and writes treated the same
- ⚠️ **No standard for Value Objects** - developers may still use primitives
- ⚠️ **Domain Events not emphasized** - typically added as an afterthought

---

### ⬡ Hexagonal Architecture (Ports & Adapters)

**Creator**: Alistair Cockburn (2005)

**Visual Structure**:
```
          ┌──────────────┐
          │  REST API    │ (Adapter)
          │  (HTTP)      │
          └──────┬───────┘
                 │
         ┌───────▼────────┐
         │  Input Port    │
         │  (Interface)   │
         └───────┬────────┘
                 │
    ┌────────────▼─────────────┐
    │     Application Core     │
    │   (Business Logic)       │
    │   Domain + Use Cases     │
    └────────────┬─────────────┘
                 │
         ┌───────▼────────┐
         │  Output Port   │
         │  (Interface)   │
         └───────┬────────┘
                 │
          ┌──────▼───────┐
          │  Database    │ (Adapter)
          │  Adapter     │
          └──────────────┘
```
**Key Characteristics**:
- **Ports** = Interfaces (define what the application needs/exposes)
- **Adapters** = Implementations (connect to external systems)
- All external systems are **pluggable**
- Core is isolated in the **hexagon center**

**In Our Codebase Example** (see `2_Hexagonal` folder):
```csharp
// Hexagonal: Port (Interface in Core)
public interface IMachineRepository
{
    Task<Machine> GetByIdAsync(string id);
    Task SaveAsync(Machine machine);
}

// Adapter (Implementation in Infrastructure)
public class SqlMachineRepository : IMachineRepository
{
    private readonly DbContext _context;
    
    public async Task<Machine> GetByIdAsync(string id)
    {
        // SQL Server implementation
        return await _context.Machines.FindAsync(id);
    }
}
```

**Strengths**:
- ✅ **Symmetry**: Input and output are both treated as adapters
- ✅ **Testability**: Easy to swap real adapters with test doubles
- ✅ **Framework agnostic**: Can plug in any technology

**Limitations**:
- ⚠️ **No clear separation between Domain and Application logic**
- ⚠️ **CQRS not built-in** - must be added manually
- ⚠️ **Doesn't prescribe internal structure** - developers choose their own patterns
- ⚠️ **Value Objects and Domain Events** - not emphasized in the pattern

---

### 🎯 Clean Architecture

**Creator**: Robert C. Martin (Uncle Bob) (2012)

**Visual Structure** (Concentric Circles):
```
    ┌────────────────────────────────────┐
    │   Frameworks & Drivers (Outermost) │
    │  (Web, DB, External Interfaces)    │
    │  ┌──────────────────────────────┐  │
    │  │  Interface Adapters          │  │
    │  │  (Controllers, Presenters,   │  │
    │  │   Gateways)                  │  │
    │  │  ┌────────────────────────┐  │  │
    │  │  │  Use Cases             │  │  │
    │  │  │  (Application Rules)   │  │  │
    │  │  │  ┌──────────────────┐  │  │  │
    │  │  │  │  Entities        │  │  │  │
    │  │  │  │  (Enterprise     │  │  │  │
    │  │  │  │   Business Rules)│  │  │  │
    │  │  │  └──────────────────┘  │  │  │
    │  │  └────────────────────────┘  │  │
    │  └──────────────────────────────┘  │
    └────────────────────────────────────┘
```

**Key Characteristics**:
- **Explicit layers**: Entities (Core) → Use Cases → Interface Adapters → Frameworks
- **Use Cases are first-class** - separate from domain entities
- **CQRS-friendly** by design
- **Emphasizes patterns**: Value Objects, Domain Events, Specifications

**In Our Codebase Example** (see `4_Clean_10` folder):

```csharp
// 1. Core (Entities) - Pure domain logic
public class Machine : EntityBase<Machine, MachineId>, IAggregateRoot
{
    public MachineName Name { get; private set; }  // Value Object
    public MachineStatus Status { get; private set; }  // SmartEnum
    
    public Machine AddProduction(int quantity, DateTime producedAt, string? batchNumber)
    {
        if (!Status.CanRecordProduction)  // Business rule in domain
            throw new InvalidOperationException(...);
        
        var production = new MachineProduction(quantity, producedAt, DateTime.UtcNow, batchNumber);
        _productions.Add(production);
        
        RegisterDomainEvent(new MachineProductionAddedEvent(this, production));  // Domain Event
        return this;
    }
}

// 2. Use Cases - Application logic (separate from domain)
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

// 3. Interface Adapters - Web layer
public class AddProduction : Endpoint<AddProductionRequest, ...>
{
    public override async Task ExecuteAsync(...)
    {
        var command = new AddProductionCommand(
            request.MachineId,
            request.Quantity,
            request.ProducedAt,
            request.BatchNumber);
        
        var result = await _mediator.Send(command);
        // Only HTTP concerns - no business logic
    }
}

// 4. Infrastructure - External concerns
public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        // EF Core mappings - completely separate from domain
        builder.Property(e => e.Name).HasVogenConversion();
    }
}
```

**Strengths**:
- ✅ **Explicit Use Cases layer** - clear separation of domain from application logic
- ✅ **CQRS built-in** - Commands and Queries are natural patterns
- ✅ **Value Objects emphasized** - eliminates primitive obsession
- ✅ **Domain Events as first-class citizens** - loose coupling by default
- ✅ **Testability** - each layer can be tested independently
- ✅ **Rich patterns library** - Specifications, Result objects, Guard clauses

**Limitations**:
- ⚠️ **Steeper learning curve** - more concepts to understand
- ⚠️ **More boilerplate** - more files and layers than simpler architectures
- ⚠️ **Can be overkill** for simple CRUD applications

---

## 🆚 Side-by-Side Comparison

### Example: Deleting a Machine

#### ❌ **Layered Architecture**
```csharp
// Business logic in SERVICE layer (scattered)
public class MachineService
{
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
}
```
**Problem**: Business rules in service layer, not testable without database.

---

#### 🧅 **Onion Architecture**
```csharp
// Domain layer
public class Machine
{
    public void EnsureCanBeDeleted(DateTime now)
    {
        if (_productions.Any(p => p.CreatedAt >= now.AddDays(-30)))
            throw new DomainException("Machine has recent production");
    }
}

// Application Service (no separate Use Cases layer)
public class MachineService
{
    public async Task DeleteMachine(int id)
    {
        var machine = await _repo.GetByIdAsync(id);
        machine.EnsureCanBeDeleted(DateTime.UtcNow);  // Domain enforces rules
        await _repo.DeleteAsync(machine);
    }
}
```
**Better**: Business rules in domain, but **application logic mixed with domain service**.

---

#### ⬡ **Hexagonal Architecture**
```csharp
// Core (Domain + Application together)
public class Machine
{
    public bool CanBeDeleted(DateTime now) =>
        !Productions.Any(p => p.CreatedAt >= now.AddDays(-30));
}

// Application Service (Port)
public interface IDeleteMachineUseCase
{
    Task<bool> ExecuteAsync(String machineId);
}

// Implementation
public class DeleteMachineUseCase : IDeleteMachineUseCase
{
    public async Task<bool> ExecuteAsync(string machineId)
    {
        var machine = await _machinePort.GetByIdAsync(machineId);
        
        if (!machine.CanBeDeleted(DateTime.UtcNow))
            return false;
        
        await _machinePort.DeleteAsync(machine);
        return true;
    }
}
```
**Good**: Ports and Adapters clear, but **Use Cases not as explicit** as Clean Architecture.

---

#### 🎯 **Clean Architecture**
```csharp
// 1. ENTITIES (Core) - Pure business rules
public class Machine : EntityBase<Machine, MachineId>
{
    public MachineStatus Status { get; private set; }  // SmartEnum with rules
    
    public void EnsureCanBeDeleted()
    {
        // Business rule: Status must allow deletion
        if (!Status.CanBeDeleted)
            throw new InvalidOperationException(
                $"Machine in {Status.Name} status cannot be deleted");
        
        // Business rule: No recent production
        if (HasRecentProduction())
            throw new InvalidOperationException(
                "Cannot delete machine with production in last 30 days");
    }
    
    public bool HasRecentProduction(int days = 30) =>
        _productions.Any(p => p.ProducedAt >= DateTime.UtcNow.AddDays(-days));
}

// 2. USE CASES - Application orchestration (separate layer)
public record DeleteMachineCommand(int MachineId) : ICommand<Result>;

public class DeleteMachineHandler : ICommandHandler<DeleteMachineCommand, Result>
{
    private readonly IRepository<Machine> _repository;
    
    public async ValueTask<Result> Handle(DeleteMachineCommand command, ...)
    {
        var machineId = MachineId.From(command.MachineId);  // Value Object
        var spec = new MachineByIdSpec(machineId);  // Specification pattern
        
        var machine = await _repository.FirstOrDefaultAsync(spec, ...);
        if (machine is null) return Result.NotFound("Machine not found");
        
        // Domain enforces rules - Use Case just orchestrates
        machine.EnsureCanBeDeleted();
        
        await _repository.DeleteAsync(machine, ...);
        return Result.Success();
    }
}

// 3. INTERFACE ADAPTERS - Web layer
public class Delete : Endpoint<DeleteMachineRequest, ...>
{
    public override async Task ExecuteAsync(...)
    {
        var result = await _mediator.Send(new DeleteMachineCommand(request.MachineId));
        return result.ToDeleteResult();  // HTTP concerns only
    }
}
```

**Best**: 
- ✅ Business rules **100% in domain**
- ✅ Use Case **explicitly orchestrates** without duplicating logic
- ✅ **Value Objects** prevent primitive obsession
- ✅ **Specification pattern** for queries
- ✅ **Result pattern** for error handling
- ✅ Web layer **completely decoupled**

---

## 📊 Key Differences Summary

| Aspect | Onion | Hexagonal | **Clean** |
|--------|-------|-----------|-----------|
| **Layer Structure** | Domain → Application → Infrastructure | Core (Domain + App) → Adapters | **Entities → Use Cases → Adapters → Frameworks** |
| **Use Cases** | No explicit layer | Implicit in core | **✅ Explicit first-class layer** |
| **CQRS Support** | Add manually | Add manually | **✅ Built-in (Commands/Queries)** |
| **Value Objects** | Optional | Optional | **✅ Core pattern, emphasized** |
| **Domain Events** | Can add | Can add | **✅ First-class citizens** |
| **Specifications** | Rarely used | Can add | **✅ Standard pattern** |
| **Testing** | Good | Good | **✅ Excellent (pure domain + use cases)** |
| **Complexity** | Medium | Medium | High (but justified for complex domains) |
| **Prescriptive Patterns** | Somewhat | Minimal | Many |
| **Learning Curve** | Easy | Moderate | Steep but worth it |
| **Best For** | Simple CRUD | DDD apps | Multi-interface apps | **Complex business domains** |

---

## 💡 When to Use Clean Architecture

### ✅ **Use Clean Architecture when**:
1. **Complex Business Logic**: Your application has sophisticated business rules
2. **Long-term Maintenance**: Project will be maintained for years
3. **Multiple Interfaces**: Need to support Web API, gRPC, CLI, etc.
4. **Team Size**: Medium to large teams benefit from clear boundaries
5. **Testability Critical**: High test coverage required
6. **Framework Flexibility**: May need to change frameworks/databases

### ❌ **Don't use Clean Architecture for**:
1. **Simple CRUD**: Basic Create-Read-Update-Delete operations
2. **Prototypes**: Rapid prototyping where architecture overhead slows you down
3. **Small Scripts**: Utility scripts or one-off tools
4. **Tight Deadlines**: When time-to-market is more important than maintainability

---

## 📚 Key Takeaways

1. **Dependency Rule**: Dependencies point inward. Core has zero dependencies.

2. **Business Logic in Core**: All business rules live in the domain, not services or controllers.

3. **Value Objects**: Strongly-typed domain concepts prevent primitive obsession.

4. **Domain Events**: Enable loose coupling and side effects without domain contamination.

5. **CQRS**: Separate reads (optimized queries) from writes (domain model).

6. **Testability**: Pure domain logic can be tested without infrastructure.

7. **Framework Independence**: Core doesn't depend on ASP.NET, EF Core, or any framework.

8. **Flexibility**: Easy to swap databases, add new interfaces, or change frameworks.

---

## 🚀 Clean Architecture in .NET 10

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

**Clean Architecture** stands out from Onion and Hexagonal architectures by:
- ✅ Making **Use Cases explicit** (not mixed with domain or infrastructure)
- ✅ Having **CQRS built-in** (Commands and Queries as natural patterns)
- ✅ **Emphasizing rich domain patterns** (Value Objects, Domain Events, Specifications)
- ✅ Providing **clear guidance** on where code belongs

While **Onion** and **Hexagonal** solve the dependency problem, **Clean Architecture** goes further by prescribing **battle-tested patterns** that lead to maintainable, testable, and flexible code.

For machine monitoring systems with complex business rules, regulatory requirements, and long-term maintenance needs, this investment pays significant dividends.

The separation of concerns ensures that:
- ✅ Business rules are **explicit and enforceable**
- ✅ Tests are **fast and comprehensive**
- ✅ Changes are **isolated and safe**
- ✅ Code is **self-documenting**
- ✅ Architecture is **future-proof**

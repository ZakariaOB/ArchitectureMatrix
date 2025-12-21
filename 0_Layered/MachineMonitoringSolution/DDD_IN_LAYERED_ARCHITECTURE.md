# Domain-Driven Design (DDD) in Layered Architecture

## Overview

This document demonstrates how **Domain-Driven Design (DDD) principles can dramatically improve even a traditional Layered Architecture**. While DDD is often associated with Clean Architecture, Hexagonal, or Onion architectures, its principles are **architecture-agnostic** and provide value in any structure.

---

## ?? What is Domain-Driven Design (DDD)?

**Domain-Driven Design** is an approach to software development that:

1. **Focuses on the core business domain** and domain logic
2. **Uses a model** that reflects deep understanding of the business
3. **Collaborates with domain experts** to refine the model
4. **Embeds business rules** in the domain objects themselves

### Key DDD Concepts

| Concept | Description | Benefit |
|---------|-------------|---------|
| **Entities** | Objects with identity and lifecycle | Track things that change over time |
| **Value Objects** | Objects defined by their attributes | Type-safe, immutable domain concepts |
| **Aggregates** | Cluster of entities with consistency boundary | Maintain invariants |
| **Domain Events** | Significant business occurrences | Loose coupling, audit trail |
| **Repositories** | Abstract data access | Domain doesn't depend on persistence |
| **Domain Services** | Operations that don't belong to entities | Business logic without state |

---

## ?? Why DDD Matters (Even in Layered Architecture)

### ? Traditional Layered Architecture WITHOUT DDD

```csharp
// Controller - Business logic in presentation layer
[HttpPost("orders/{id}/confirm")]
public async Task<IActionResult> ConfirmOrder(int id)
{
    var order = await _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.OrderId == id);
    
    // ? Business rules scattered in controller
    if (order.Status != "Pending")
        return BadRequest("Order must be pending");
    
    if (!order.Lines.Any())
        return BadRequest("Order must have lines");
    
    if (order.Lines.Sum(l => l.Quantity * l.UnitPrice) <= 0)
        return BadRequest("Total must be positive");
    
    // ? Direct database manipulation
    order.Status = "Confirmed";
    order.ConfirmedDate = DateTime.UtcNow;
    
    await _db.SaveChangesAsync();
    
    // ? Side effects mixed with business logic
    await _emailService.SendOrderConfirmation(order.CustomerEmail);
    
    return Ok();
}
```

**Problems**:
- ? Business rules duplicated across controllers
- ? Rules can be bypassed
- ? Impossible to test without HTTP context and database
- ? No protection from invalid states
- ? Changes are hard to audit
- ? Side effects tightly coupled

---

### ? Layered Architecture WITH DDD

```csharp
// Domain Model - Business logic in entity
public class Order
{
    public OrderId Id { get; private set; }
    public string CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderLine> _lines = new List<OrderLine>();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    
    // ? Business rules enforced in domain
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order must be pending");
        
        if (!_lines.Any())
            throw new InvalidOperationException("Order must have lines");
        
        if (GetTotalAmount() <= 0)
            throw new InvalidOperationException("Total must be positive");
        
        ChangeStatus(OrderStatus.Confirmed);
        
        // ? Domain event for side effects
        RaiseDomainEvent(new OrderConfirmedEvent(Id, DateTime.UtcNow, GetTotalAmount()));
    }
}

// Controller - Thin, just orchestrates
[HttpPost("orders/{id}/confirm")]
public async Task<IActionResult> ConfirmOrder(int id)
{
    var order = await _orderService.GetOrderById(id);
    if (order == null) return NotFound();
    
    // ? Domain enforces rules
    order.Confirm();
    
    await _orderService.SaveOrder(order);
    
    return Ok();
}

// Event Handler - Side effects decoupled
public class OrderConfirmedEventHandler
{
    public async Task Handle(OrderConfirmedEvent @event)
    {
        // Send email notification
        await _emailService.SendOrderConfirmation(@event.OrderId);
    }
}
```

**Benefits**:
- ? Business rules centralized in domain
- ? Rules cannot be bypassed
- ? Testable without database/HTTP
- ? Invalid states impossible
- ? Changes auditable via events
- ? Side effects loosely coupled

---

## ?? The Order Example

Our example implements a **Purchase Order** system with DDD principles:

### Domain Model Structure

```
MachineMonitoringRepository/Models/DDD/
??? Order.cs              # Aggregate Root (Entity)
??? OrderId.cs            # Value Object
??? OrderStatus.cs        # Value Object (Rich Enum)
??? OrderLine.cs          # Value Object
??? OrderEvents.cs        # Domain Events
```

---

## ??? DDD Building Blocks Explained

### 1. Value Objects

**Problem**: Primitive obsession makes code unclear and unsafe.

```csharp
// ? Without Value Objects (Primitive Obsession)
public class Order
{
    public int OrderId { get; set; }  // Just an int - no validation
    public string Status { get; set; }  // Magic strings - typos possible
}

// Dangerous code:
int orderId = -1;  // Invalid but compiles
string status = "Confrimed";  // Typo, but compiles
```

```csharp
// ? With Value Objects
public class Order
{
    public OrderId Id { get; private set; }  // Type-safe, validated
    public OrderStatus Status { get; private set; }  // Constrained values
}

// Compile-time safety:
OrderId id = OrderId.Create(-1);  // Throws ArgumentException
OrderStatus status = OrderStatus.FromString("Confrimed");  // Throws ArgumentException
```

**Benefits**:
- ? **Type Safety**: Can't mix up different IDs
- ? **Validation**: Business rules enforced at construction
- ? **Immutability**: Once created, cannot be changed
- ? **Self-Documenting**: `OrderId` is clearer than `int`

### 2. Rich Domain Model (Entities)

**Problem**: Anemic domain model with business logic elsewhere.

```csharp
// ? Anemic Domain Model
public class Order
{
    public int OrderId { get; set; }
    public string Status { get; set; }  // Public setter - anyone can change
    public List<OrderLine> Lines { get; set; }  // Public setter - no protection
}

// Business logic in service layer
public class OrderService
{
    public void ConfirmOrder(Order order)
    {
        // ? Business rules scattered
        if (order.Status != "Pending") throw new Exception();
        if (!order.Lines.Any()) throw new Exception();
        order.Status = "Confirmed";  // Direct property manipulation
    }
}
```

```csharp
// ? Rich Domain Model
public class Order
{
    private readonly List<OrderLine> _lines = new List<OrderLine>();
    public OrderStatus Status { get; private set; }  // Protected
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();  // Encapsulated
    
    // ? Business logic in entity
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order must be pending");
        
        if (!_lines.Any())
            throw new InvalidOperationException("Order must have lines");
        
        ChangeStatus(OrderStatus.Confirmed);
    }
    
    public void AddLine(OrderLine line)
    {
        if (!Status.CanModifyOrder())
            throw new InvalidOperationException("Cannot modify order");
        
        _lines.Add(line);
    }
}
```

**Benefits**:
- ? **Encapsulation**: Private setters protect invariants
- ? **Behavior**: Methods express business operations
- ? **Validation**: Rules enforced by entity itself
- ? **Consistency**: Always in valid state

### 3. Domain Events

**Problem**: Tight coupling between business logic and side effects.

```csharp
// ? Without Domain Events (Tight Coupling)
public void ConfirmOrder(Order order)
{
    order.Status = "Confirmed";
    
    // ? Tightly coupled side effects
    _emailService.SendConfirmation(order);
    _inventoryService.ReserveItems(order);
    _analyticsService.TrackOrderConfirmed(order);
    _notificationService.NotifyWarehouse(order);
}
```

```csharp
// ? With Domain Events (Loose Coupling)
public class Order
{
    private readonly List<object> _domainEvents = new List<object>();
    
    public void Confirm()
    {
        ChangeStatus(OrderStatus.Confirmed);
        
        // ? Raise event - handlers execute independently
        _domainEvents.Add(new OrderConfirmedEvent(Id, DateTime.UtcNow, GetTotalAmount()));
    }
}

// Event handlers execute independently
public class SendOrderConfirmationHandler : IHandle<OrderConfirmedEvent>
{
    public async Task Handle(OrderConfirmedEvent @event)
    {
        await _emailService.SendConfirmation(@event.OrderId);
    }
}

public class ReserveInventoryHandler : IHandle<OrderConfirmedEvent>
{
    public async Task Handle(OrderConfirmedEvent @event)
    {
        await _inventoryService.Reserve(@event.OrderId);
    }
}
```

**Benefits**:
- ? **Loose Coupling**: Entity doesn't know about side effects
- ? **Extensibility**: Add new handlers without changing entity
- ? **Audit Trail**: Events document what happened
- ? **Async Processing**: Handlers can run asynchronously

### 4. Aggregates & Consistency Boundaries

**Aggregate**: Cluster of entities and value objects with a consistency boundary.

```csharp
// ? Order is an Aggregate Root
public class Order  // Aggregate Root
{
    private readonly List<OrderLine> _lines = new List<OrderLine>();  // Part of aggregate
    
    // ? Aggregate enforces invariants
    public void AddLine(OrderLine line)
    {
        // Business rule: Lines must sum to positive total
        _lines.Add(line);
        
        if (GetTotalAmount() < 0)
        {
            _lines.Remove(line);
            throw new InvalidOperationException("Total cannot be negative");
        }
    }
}
```

**Rules**:
1. **External objects** can only reference the aggregate root (Order)
2. **Internal objects** (OrderLines) are accessed through the root
3. **Invariants** are maintained by the aggregate root

---

## ?? Testing Benefits

### Pure Domain Logic Tests

```csharp
[Fact]
public void Order_Confirm_WithLines_ConfirmsOrder()
{
    // Arrange - Pure domain objects, NO mocking!
    var order = Order.Create("John Doe");
    order.AddLine(OrderLine.Create("Widget", 5, 10.00m));

    // Act - Pure business logic, NO database/HTTP
    order.Confirm();

    // Assert
    Assert.Equal(OrderStatus.Confirmed, order.Status);
}

[Fact]
public void Order_Confirm_WithoutLines_ThrowsException()
{
    // Arrange
    var order = Order.Create("John Doe");

    // Act & Assert
    var exception = Assert.Throws<InvalidOperationException>(() => order.Confirm());
    Assert.Contains("without any lines", exception.Message);
}
```

**Benefits**:
- ? **Fast**: No I/O, no database, no HTTP
- ? **Simple**: No mocking frameworks needed
- ? **Clear**: Tests document business rules
- ? **Reliable**: No external dependencies can fail

---

## ?? Before and After Comparison

| Aspect | Without DDD | With DDD |
|--------|-------------|----------|
| **Business Rules** | Scattered across layers | Centralized in domain |
| **Testing** | Needs database/HTTP | Pure unit tests |
| **Type Safety** | Primitive types everywhere | Value Objects |
| **Validation** | Manual checks | Built into types |
| **State Protection** | Public setters | Private setters + methods |
| **Side Effects** | Tightly coupled | Domain Events |
| **Audit Trail** | Manual logging | Event history |
| **Code Clarity** | Implicit rules | Explicit behavior |

---

## ?? Running the Example

### 1. Run the Tests

```bash
cd 0_Layered/MachineMonitoringSolution
dotnet test --filter "FullyQualifiedName~DDD"
```

You'll see tests passing for:
- Order creation with validation
- Adding/removing lines with business rules
- Status transitions with constraints
- Value Object equality and immutability
- Domain events being raised

### 2. Key Tests to Review

| Test File | Demonstrates |
|-----------|-------------|
| `OrderTests.cs` | Rich domain model with business logic |
| `ValueObjectTests.cs` | Type safety and validation |

---

## ?? Key Takeaways

### 1. **DDD is Architecture-Agnostic**

DDD principles work in:
- ? Traditional Layered Architecture (this example!)
- ? Clean Architecture
- ? Hexagonal Architecture
- ? Onion Architecture

### 2. **Start with Value Objects**

Easiest DDD win:
```csharp
// Before: public int OrderId { get; set; }
// After:  public OrderId Id { get; private set; }
```

### 3. **Rich Domain Model > Anemic Model**

```csharp
// ? Anemic: order.Status = "Confirmed";
// ? Rich:    order.Confirm();
```

### 4. **Domain Events for Side Effects**

```csharp
// ? Tight: ConfirmOrder() calls email, inventory, etc.
// ? Loose: ConfirmOrder() raises event, handlers react
```

### 5. **Test Business Logic Directly**

```csharp
// ? Integration test with database
// ? Unit test with pure domain objects
```

---

## ?? How DDD Improves Layered Architecture

Even in traditional layered architecture, DDD provides:

### ? **Better Code Organization**
- Business logic in one place (domain entities)
- Not scattered across controllers/services

### ? **Testability**
- Fast unit tests without database
- Business rules explicitly tested

### ? **Type Safety**
- Value Objects prevent primitive obsession
- Compile-time checks for business rules

### ? **Maintainability**
- Changes to business rules in one place
- Domain Events for audit trail

### ? **Protection**
- Encapsulation prevents invalid states
- Business rules cannot be bypassed

---

## ?? Further Reading

- **Book**: *Domain-Driven Design* by Eric Evans
- **Book**: *Implementing Domain-Driven Design* by Vaughn Vernon
- **Article**: [Anemic Domain Model](https://martinfowler.com/bliki/AnemicDomainModel.html) by Martin Fowler
- **Video**: [Domain-Driven Design Fundamentals](https://www.pluralsight.com/courses/domain-driven-design-fundamentals)

---

## ?? For Your Presentation

### Key Points to Emphasize:

1. **DDD is NOT an architecture** - it's a design approach that works with any architecture

2. **Even layered architecture benefits** - as demonstrated by our Order example

3. **Start small** - Begin with Value Objects, then move to rich entities

4. **Tests prove the value** - Show how domain logic tests are simpler and faster

5. **Compare before/after** - Use the code examples showing primitive vs. Value Object

6. **Live demo** - Run the tests to show they work without database/HTTP

---

## Conclusion

**Domain-Driven Design improves ANY architecture** by:
- ? Centralizing business logic in the domain
- ? Making business rules explicit and testable
- ? Preventing invalid states through encapsulation
- ? Enabling loose coupling through domain events

Even in a traditional layered architecture, these benefits are substantial. DDD is about **how you model your domain**, not what outer architecture you choose.

The Order example demonstrates that with DDD, even layered architecture can have:
- Clean, testable business logic
- Type-safe domain concepts
- Protected invariants
- Auditable changes

**The key insight**: You don't need Clean Architecture to get DDD benefits. Start applying DDD principles in your current architecture today!

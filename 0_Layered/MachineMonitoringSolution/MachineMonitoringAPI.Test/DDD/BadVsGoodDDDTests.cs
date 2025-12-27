using MachineMonitoringRepository.Models.DDD;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineMonitoring.Tests.DDD
{
    /// <summary>
    /// DEMONSTRATES: BAD (without DDD) vs GOOD (with DDD) implementations
    /// 
    /// This test class shows the DRAMATIC difference between:
    /// 1. Traditional layered architecture WITHOUT DDD principles
    /// 2. DDD-based architecture WITH value objects and aggregates
    /// 
    /// KEY PROBLEMS WITHOUT DDD (BAD):
    /// ? Business logic scattered across controllers/services
    /// ? Primitive obsession (using strings, ints everywhere)
    /// ? No validation at domain level
    /// ? Easy to create invalid states
    /// ? Hard to test business rules
    /// ? Difficult to maintain consistency
    /// 
    /// KEY BENEFITS WITH DDD (GOOD):
    /// ? Business logic encapsulated in domain
    /// ? Type safety with value objects
    /// ? Validation happens at construction
    /// ? Impossible to create invalid states
    /// ? Easy to test without database
    /// ? Consistency guaranteed by aggregate
    /// </summary>
    public class BadVsGoodDDDTests
    {
        #region BAD APPROACH - Without DDD

        /// <summary>
        /// BAD: Using primitives and anemic domain model
        /// PROBLEMS:
        /// - No validation
        /// - Easy to create invalid data
        /// - Business rules not enforced
        /// - Status is just a string (typos possible)
        /// </summary>
        [Fact]
        public void BAD_PrimitiveObsession_AllowsInvalidData()
        {
            // BAD: Using primitives everywhere
            int orderId = -1; // ? Negative ID is invalid but allowed
            string customerName = ""; // ? Empty name is invalid but allowed
            string status = "Pendng"; // ? Typo! Should be "Pending"
            decimal totalAmount = -100; // ? Negative total is invalid but allowed
            
            // BAD: Creating order line with invalid data
            var orderLine = new BadOrderLine
            {
                ProductName = "", // ? Empty product name
                Quantity = -5, // ? Negative quantity
                UnitPrice = -10 // ? Negative price
            };
            
            // ? ALL OF THIS COMPILES AND RUNS!
            // No validation, no protection, no safety
            Assert.True(orderId < 0); // This is wrong but allowed
            Assert.True(string.IsNullOrEmpty(customerName)); // This is wrong but allowed
            Assert.Equal("Pendng", status); // Typo but allowed
            Assert.True(totalAmount < 0); // This is wrong but allowed
        }

        /// <summary>
        /// BAD: Business logic in controller/service layer
        /// PROBLEMS:
        /// - Logic scattered everywhere
        /// - Hard to test
        /// - Easy to bypass validation
        /// - Duplication of rules
        /// </summary>
        [Fact]
        public void BAD_BusinessLogicInController_ScatteredAndDuplicated()
        {
            // Simulating controller/service logic WITHOUT domain encapsulation
            var badOrder = new BadOrder
            {
                OrderId = 1,
                CustomerName = "John Doe",
                Status = "Pending",
                Lines = new List<BadOrderLine>()
            };
            
            // ? Business rule #1: "Can only add lines when Pending or Confirmed"
            // This check must be repeated in EVERY place that adds lines
            if (badOrder.Status != "Pending" && badOrder.Status != "Confirmed")
            {
                // What happens here? Exception? Return false? Different in each place!
            }
            
            badOrder.Lines.Add(new BadOrderLine 
            { 
                ProductName = "Widget", 
                Quantity = 5, 
                UnitPrice = 10 
            });
            
            // ? Business rule #2: "Calculate total"
            // This calculation must be repeated everywhere it's needed
            decimal total = badOrder.Lines.Sum(l => l.Quantity * l.UnitPrice);
            
            // ? Business rule #3: "Can only confirm if has lines"
            // This check must be repeated in every service method
            if (badOrder.Status == "Pending")
            {
                if (badOrder.Lines.Any())
                {
                    badOrder.Status = "Confirmed"; // ? Direct property manipulation
                }
            }
            
            // ? PROBLEMS:
            // - Rules are scattered across controllers/services
            // - Easy to forget validation
            // - Hard to test (need full stack)
            // - Different developers implement differently
            // - No single source of truth
            
            Assert.Equal("Confirmed", badOrder.Status);
        }

        /// <summary>
        /// BAD: Invalid state transitions are allowed
        /// PROBLEMS:
        /// - No enforcement of valid transitions
        /// - Can jump to any status
        /// - Data corruption possible
        /// </summary>
        [Fact]
        public void BAD_InvalidStateTransitions_Allowed()
        {
            var badOrder = new BadOrder
            {
                OrderId = 1,
                CustomerName = "John Doe",
                Status = "Pending",
                Lines = new List<BadOrderLine>()
            };
            
            // ? INVALID: Jump directly from Pending to Completed (should go through Confirmed -> InProduction)
            badOrder.Status = "Completed"; 
            Assert.Equal("Completed", badOrder.Status); // This is WRONG but allowed!
            
            // ? INVALID: Change Completed order back to Pending (terminal state violated)
            badOrder.Status = "Pending";
            Assert.Equal("Pending", badOrder.Status); // This is WRONG but allowed!
            
            // ? INVALID: Modify completed order
            badOrder.Lines.Add(new BadOrderLine 
            { 
                ProductName = "Widget", 
                Quantity = 1, 
                UnitPrice = 10 
            });
            
            // ? NO PROTECTION! Data integrity is violated!
        }

        /// <summary>
        /// BAD: Testing requires full infrastructure
        /// PROBLEMS:
        /// - Need database to test business rules
        /// - Slow tests
        /// - Complex setup
        /// </summary>
        [Fact]
        public void BAD_TestingRequiresFullStack()
        {
            // To test business rules with anemic model, you need:
            // ? Database context
            // ? Service layer
            // ? Repository layer
            // ? Potentially HTTP context, auth, etc.
            
            // Example: To test "can confirm order with lines"
            // You would need to:
            // 1. Setup in-memory database
            // 2. Create DbContext
            // 3. Create repository
            // 4. Create service with dependencies
            // 5. Mock additional services (email, etc.)
            // 6. Finally test the business rule
            
            // This is SLOW, COMPLEX, and BRITTLE
            
            Assert.True(true); // Placeholder - actual test would be very complex
        }

        #endregion

        #region GOOD APPROACH - With DDD

        /// <summary>
        /// GOOD: Value objects prevent invalid data at compile time
        /// BENEFITS:
        /// ? Type safety
        /// ? Validation at construction
        /// ? Impossible to create invalid objects
        /// ? Self-documenting code
        /// </summary>
        [Fact]
        public void GOOD_ValueObjects_PreventInvalidData()
        {
            // ? GOOD: Value objects with validation
            
            // ? Cannot create invalid OrderId
            Assert.Throws<ArgumentException>(() => OrderId.Create(-1));
            Assert.Throws<ArgumentException>(() => OrderId.Create(0));
            
            var validId = OrderId.Create(1); // ? Only valid IDs can be created
            Assert.Equal(1, validId.Value);
            
            // ? Cannot create invalid OrderLine
            Assert.Throws<ArgumentException>(() => 
                OrderLine.Create("", 5, 10)); // Empty product name
            Assert.Throws<ArgumentException>(() => 
                OrderLine.Create("Widget", -5, 10)); // Negative quantity
            Assert.Throws<ArgumentException>(() => 
                OrderLine.Create("Widget", 5, -10)); // Negative price
            
            var validLine = OrderLine.Create("Widget", 5, 10.00m); // ? Only valid lines
            Assert.Equal("Widget", validLine.ProductName);
            Assert.Equal(5, validLine.Quantity);
            Assert.Equal(50.00m, validLine.TotalPrice); // ? Calculated automatically
            
            // ? Cannot create invalid OrderStatus
            Assert.Throws<ArgumentException>(() => 
                OrderStatus.FromString("Pendng")); // Typo caught!
            
            var validStatus = OrderStatus.FromString("Pending"); // ? Only valid statuses
            Assert.Equal(OrderStatus.Pending, validStatus);
            
            // ? TYPE SAFETY: Cannot pass wrong types
            // orderId = customerName; // ? Compile error! Type safety!
            // status = totalAmount; // ? Compile error! Type safety!
        }

        /// <summary>
        /// GOOD: Business logic encapsulated in aggregate
        /// BENEFITS:
        /// ? Single source of truth for rules
        /// ? Impossible to bypass validation
        /// ? Consistent behavior everywhere
        /// ? Easy to understand and maintain
        /// </summary>
        [Fact]
        public void GOOD_AggregateEncapsulation_EnforceBusinessRules()
        {
            // ? GOOD: Create order through factory method
            var order = Order.Create("John Doe");
            
            // ? Business rule: Can add lines when Pending
            var line = OrderLine.Create("Widget", 5, 10.00m);
            order.AddLine(line);
            Assert.Single(order.Lines);
            
            // ? Business rule: Total calculated automatically
            Assert.Equal(50.00m, order.GetTotalAmount());
            
            // ? Business rule: Can confirm when has lines
            order.Confirm();
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            
            // ? Business rule: Cannot add lines when not modifiable
            var anotherLine = OrderLine.Create("Gadget", 2, 15.00m);
            order.StartProduction(); // Status -> InProduction
            
            var exception = Assert.Throws<InvalidOperationException>(() => 
                order.AddLine(anotherLine)); // ? Prevented!
            Assert.Contains("Cannot add lines", exception.Message);
            
            // ? BENEFITS:
            // - Rules enforced EVERYWHERE automatically
            // - No code duplication
            // - Impossible to bypass
            // - Self-documenting
            // - Easy to test
        }

        /// <summary>
        /// GOOD: State transitions enforced by aggregate
        /// BENEFITS:
        /// ? Valid transitions only
        /// ? State machine enforced
        /// ? Data integrity guaranteed
        /// </summary>
        [Fact]
        public void GOOD_StateTransitions_EnforcedByAggregate()
        {
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            
            // ? Valid transition: Pending -> Confirmed
            order.Confirm();
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            
            // ? INVALID transition: Confirmed -> Completed (must go through InProduction)
            var exception1 = Assert.Throws<InvalidOperationException>(() => 
                order.Complete());
            Assert.Contains("Cannot complete order in Confirmed status", exception1.Message);
            
            // ? Valid transition: Confirmed -> InProduction
            order.StartProduction();
            Assert.Equal(OrderStatus.InProduction, order.Status);
            
            // ? Valid transition: InProduction -> Completed
            order.Complete();
            Assert.Equal(OrderStatus.Completed, order.Status);
            Assert.NotNull(order.CompletedDate);
            
            // ? INVALID: Cannot modify completed order
            var exception2 = Assert.Throws<InvalidOperationException>(() => 
                order.Cancel("Changed mind"));
            Assert.Contains("Cannot cancel order in Completed status", exception2.Message);
            
            // ? DATA INTEGRITY MAINTAINED!
        }

        /// <summary>
        /// GOOD: Testing without infrastructure
        /// BENEFITS:
        /// ? No database needed
        /// ? Fast tests (milliseconds)
        /// ? Simple setup
        /// ? Focus on business logic
        /// </summary>
        [Fact]
        public void GOOD_TestingWithoutInfrastructure()
        {
            // ? Test business rule: Cannot confirm order without lines
            var order = Order.Create("John Doe");
            
            var exception = Assert.Throws<InvalidOperationException>(() => 
                order.Confirm());
            Assert.Contains("Cannot confirm order without any lines", exception.Message);
            
            // ? Test business rule: Can confirm order with lines
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            order.Confirm(); // ? Works!
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            
            // ? Test business rule: Same product combines quantity
            order = Order.Create("Jane Smith");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            order.AddLine(OrderLine.Create("Widget", 3, 10.00m));
            
            Assert.Single(order.Lines); // ? Combined into one line
            Assert.Equal(8, order.Lines.First().Quantity); // ? 5 + 3
            
            // ? BENEFITS:
            // - NO database needed
            // - NO services needed
            // - NO infrastructure needed
            // - Tests run in MILLISECONDS
            // - Easy to write and maintain
        }

        /// <summary>
        /// GOOD: Domain events for side effects
        /// BENEFITS:
        /// ? Separation of concerns
        /// ? Loosely coupled
        /// ? Auditable
        /// ? Extensible
        /// </summary>
        [Fact]
        public void GOOD_DomainEvents_DecouplesSideEffects()
        {
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            
            // ? Confirm order raises domain event
            order.Confirm();
            
            // ? Check domain events were raised
            Assert.NotEmpty(order.DomainEvents);
            var placedEvent = order.DomainEvents.OfType<OrderPlacedEvent>().FirstOrDefault();
            Assert.NotNull(placedEvent);
            Assert.Equal(50.00m, placedEvent.TotalAmount);
            Assert.Equal("John Doe", placedEvent.CustomerName);
            
            // ? Cancel order raises event
            order.Cancel("Customer requested");
            var cancelledEvent = order.DomainEvents.OfType<OrderCancelledEvent>().FirstOrDefault();
            Assert.NotNull(cancelledEvent);
            Assert.Equal("Customer requested", cancelledEvent.Reason);
            
            // ? BENEFITS:
            // - Domain doesn't depend on email service, logging, etc.
            // - Side effects handled by event handlers
            // - Easy to add new side effects
            // - Full audit trail
        }

        #endregion

        #region COMPARISON SUMMARY

        /// <summary>
        /// SUMMARY: Side-by-side comparison
        /// 
        /// BAD (Without DDD):
        /// ? Primitives everywhere (int, string, decimal)
        /// ? Business logic scattered in controllers/services
        /// ? Easy to create invalid data
        /// ? Hard to test (need full stack)
        /// ? Code duplication
        /// ? No type safety
        /// ? No encapsulation
        /// ? Difficult to maintain
        /// 
        /// GOOD (With DDD):
        /// ? Value objects (OrderId, OrderLine, OrderStatus)
        /// ? Business logic in domain aggregate
        /// ? Impossible to create invalid data
        /// ? Easy to test (no infrastructure needed)
        /// ? Single source of truth
        /// ? Type safety
        /// ? Full encapsulation
        /// ? Easy to maintain and extend
        /// 
        /// CONCLUSION:
        /// DDD principles (Value Objects + Aggregates) provide TREMENDOUS benefits
        /// even in a layered architecture. They make code:
        /// - Safer
        /// - Testable
        /// - Maintainable
        /// - Self-documenting
        /// - Less prone to bugs
        /// </summary>
        [Fact]
        public void Summary_DDDProvidesHugeBenefits()
        {
            // This test demonstrates the key insight:
            // You don't need to switch to hexagonal/clean architecture
            // to get DDD benefits!
            
            // ? You CAN use DDD principles in layered architecture
            // ? Value Objects work in any architecture
            // ? Aggregates work in any architecture
            // ? Domain Events work in any architecture
            
            Assert.True(true, "DDD principles improve ANY architecture!");
        }

        #endregion

        #region Helper Classes for BAD examples

        // BAD: Anemic domain model (just data, no behavior)
        private class BadOrder
        {
            public int OrderId { get; set; } // ? No validation
            public string CustomerName { get; set; } = string.Empty; // ? Can be empty
            public string Status { get; set; } = string.Empty; // ? Can be any string
            public List<BadOrderLine> Lines { get; set; } = new(); // ? Exposed as mutable
            public decimal TotalAmount { get; set; } // ? Can be set to anything
        }

        // BAD: Anemic domain model
        private class BadOrderLine
        {
            public string ProductName { get; set; } = string.Empty; // ? No validation
            public int Quantity { get; set; } // ? Can be negative
            public decimal UnitPrice { get; set; } // ? Can be negative
        }

        #endregion
    }
}

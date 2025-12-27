using Microsoft.AspNetCore.Mvc;
using MachineMonitoringRepository.Models.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineMonitoring.WebAPI.Controllers.Examples
{
    /// <summary>
    /// GOOD EXAMPLE: Controller using DDD principles
    /// 
    /// BENEFITS DEMONSTRATED:
    /// ? Business logic in domain (not in controller)
    /// ? Value objects for type safety
    /// ? Aggregate enforces invariants
    /// ? Impossible to create invalid states
    /// ? Easy to test (domain tests don't need HTTP)
    /// ? Single source of truth for rules
    /// ? Self-documenting code
    /// ? Clear separation of concerns
    /// 
    /// This controller is THIN - it only handles HTTP concerns
    /// All business logic is in the Order aggregate
    /// </summary>
    [ApiController]
    [Route("api/good/[controller]")]
    public class GoodOrderController : ControllerBase
    {
        // Simulating repository (in real code, this would inject IOrderRepository)
        private static readonly List<Order> _orderRepository = new();

        /// <summary>
        /// GOOD: Create order endpoint - thin controller
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                // ? BENEFIT 1: Domain factory method handles validation
                // No validation code in controller!
                var order = Order.Create(request.CustomerName);

                // ? BENEFIT 2: Repository handles persistence
                _orderRepository.Add(order);

                // ? BENEFIT 3: Return meaningful response
                return Ok(new
                {
                    orderId = order.Id.Value,
                    customerName = order.CustomerName,
                    status = order.Status.Value,
                    message = "Order created successfully"
                });
            }
            catch (ArgumentException ex)
            {
                // ? BENEFIT 4: Domain throws descriptive exceptions
                // Controller just translates to HTTP response
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Add line to order - domain handles all business logic
        /// </summary>
        [HttpPost("{orderId}/lines")]
        public async Task<IActionResult> AddLineToOrder(int orderId, [FromBody] AddOrderLineRequest request)
        {
            try
            {
                // ? BENEFIT 5: Find order using value object (type-safe)
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 6: Create value object (validates automatically)
                var line = OrderLine.Create(
                    request.ProductName,
                    request.Quantity,
                    request.UnitPrice);

                // ? BENEFIT 7: Domain method handles ALL business logic
                // - Validates status (can only modify if Pending/Confirmed)
                // - Checks for duplicate products
                // - Combines quantities if product exists
                // - Maintains consistency
                order.AddLine(line);

                // ? BENEFIT 8: No manual calculations needed
                // Total is calculated automatically by domain

                return Ok(new
                {
                    totalLines = order.Lines.Count,
                    totalAmount = order.GetTotalAmount(),
                    message = "Line added successfully"
                });
            }
            catch (ArgumentException ex)
            {
                // ? Validation error from value object
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // ? Business rule violation from aggregate
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Confirm order - domain enforces all rules
        /// </summary>
        [HttpPost("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 9: Single method call enforces ALL rules
                // - Must be in Pending status
                // - Must have at least one line
                // - Total must be positive
                // - Valid state transition
                // - Raises domain event
                order.Confirm();

                // ? BENEFIT 10: Handle domain events (side effects)
                await HandleDomainEvents(order);

                return Ok(new
                {
                    orderId = order.Id.Value,
                    status = order.Status.Value,
                    totalAmount = order.GetTotalAmount(),
                    message = "Order confirmed successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                // ? Domain clearly communicates why operation failed
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Start production - state machine enforced by domain
        /// </summary>
        [HttpPost("{orderId}/start-production")]
        public async Task<IActionResult> StartProduction(int orderId)
        {
            try
            {
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 11: Domain enforces valid state transitions
                // Can only start production if order is Confirmed
                order.StartProduction();

                await HandleDomainEvents(order);

                return Ok(new
                {
                    orderId = order.Id.Value,
                    status = order.Status.Value,
                    message = "Production started"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Complete order
        /// </summary>
        [HttpPost("{orderId}/complete")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            try
            {
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 12: Domain ensures order can only be completed
                // from InProduction status
                order.Complete();

                await HandleDomainEvents(order);

                return Ok(new
                {
                    orderId = order.Id.Value,
                    status = order.Status.Value,
                    completedDate = order.CompletedDate,
                    message = "Order completed"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Cancel order with reason
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 13: Domain validates if cancellation is allowed
                // Cannot cancel Completed or already Cancelled orders
                order.Cancel(request.Reason);

                // ? BENEFIT 14: Domain event includes cancellation reason
                await HandleDomainEvents(order);

                return Ok(new
                {
                    orderId = order.Id.Value,
                    status = order.Status.Value,
                    message = "Order cancelled"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Get order details
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            // ? BENEFIT 15: Rich domain model provides all calculations
            return Ok(new
            {
                orderId = order.Id.Value,
                customerName = order.CustomerName,
                status = order.Status.Value,
                orderDate = order.OrderDate,
                completedDate = order.CompletedDate,
                totalAmount = order.GetTotalAmount(), // ? Always consistent
                totalItems = order.GetTotalItems(), // ? Domain calculation
                lines = order.Lines.Select(l => new
                {
                    productName = l.ProductName,
                    quantity = l.Quantity,
                    unitPrice = l.UnitPrice,
                    totalPrice = l.TotalPrice // ? Always consistent
                })
            });
        }

        /// <summary>
        /// GOOD: Remove line from order
        /// </summary>
        [HttpDelete("{orderId}/lines/{productName}")]
        public async Task<IActionResult> RemoveLineFromOrder(int orderId, string productName)
        {
            try
            {
                var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
                if (order == null)
                {
                    return NotFound(new { error = "Order not found" });
                }

                // ? BENEFIT 16: Domain handles removal with validation
                order.RemoveLine(productName);

                return Ok(new
                {
                    totalLines = order.Lines.Count,
                    totalAmount = order.GetTotalAmount(),
                    message = "Line removed successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// GOOD: Check if order meets minimum value
        /// </summary>
        [HttpGet("{orderId}/meets-minimum/{minimumValue}")]
        public async Task<IActionResult> CheckMinimumValue(int orderId, decimal minimumValue)
        {
            var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            // ? BENEFIT 17: Domain provides business queries
            var meetsMinimum = order.MeetsMinimumOrderValue(minimumValue);

            return Ok(new
            {
                orderId = order.Id.Value,
                currentTotal = order.GetTotalAmount(),
                minimumValue,
                meetsMinimum,
                message = meetsMinimum
                    ? "Order meets minimum value"
                    : $"Order needs ${minimumValue - order.GetTotalAmount():F2} more"
            });
        }

        /// <summary>
        /// GOOD: Get available status transitions
        /// </summary>
        [HttpGet("{orderId}/available-transitions")]
        public async Task<IActionResult> GetAvailableTransitions(int orderId)
        {
            var order = _orderRepository.FirstOrDefault(o => o.Id.Value == orderId);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            // ? BENEFIT 18: Domain knows valid transitions
            var allStatuses = OrderStatus.GetAll();
            var availableTransitions = allStatuses
                .Where(status => order.Status.CanTransitionTo(status))
                .Select(status => status.Value)
                .ToList();

            return Ok(new
            {
                currentStatus = order.Status.Value,
                availableTransitions,
                canModifyOrder = order.Status.CanModifyOrder(),
                canBeCancelled = order.Status.CanBeCancelled()
            });
        }

        #region Domain Event Handling

        /// <summary>
        /// GOOD: Handle domain events (side effects)
        /// ? Decoupled from domain logic
        /// ? Easy to add new handlers
        /// ? Provides audit trail
        /// </summary>
        private async Task HandleDomainEvents(Order order)
        {
            foreach (var domainEvent in order.DomainEvents)
            {
                // ? BENEFIT 19: Type-safe event handling
                switch (domainEvent)
                {
                    case OrderPlacedEvent placedEvent:
                        // Send confirmation email
                        await SendOrderConfirmationEmail(placedEvent);
                        // Reserve inventory
                        await ReserveInventory(placedEvent);
                        // Log to audit trail
                        Console.WriteLine($"Event: {placedEvent}");
                        break;

                    case OrderStatusChangedEvent statusEvent:
                        // Update external systems
                        await NotifyExternalSystems(statusEvent);
                        // Log to audit trail
                        Console.WriteLine($"Event: {statusEvent}");
                        break;

                    case OrderCancelledEvent cancelledEvent:
                        // Release inventory
                        await ReleaseInventory(cancelledEvent);
                        // Notify customer
                        await SendCancellationEmail(cancelledEvent);
                        // Log to audit trail
                        Console.WriteLine($"Event: {cancelledEvent}");
                        break;
                }
            }

            // ? BENEFIT 20: Clear events after handling
            order.ClearDomainEvents();
        }

        // Simulated side effect handlers
        private async Task SendOrderConfirmationEmail(OrderPlacedEvent evt)
        {
            // In real app: await _emailService.SendConfirmation(...)
            await Task.CompletedTask;
        }

        private async Task ReserveInventory(OrderPlacedEvent evt)
        {
            // In real app: await _inventoryService.Reserve(...)
            await Task.CompletedTask;
        }

        private async Task NotifyExternalSystems(OrderStatusChangedEvent evt)
        {
            // In real app: await _integrationService.Notify(...)
            await Task.CompletedTask;
        }

        private async Task ReleaseInventory(OrderCancelledEvent evt)
        {
            // In real app: await _inventoryService.Release(...)
            await Task.CompletedTask;
        }

        private async Task SendCancellationEmail(OrderCancelledEvent evt)
        {
            // In real app: await _emailService.SendCancellation(...)
            await Task.CompletedTask;
        }

        #endregion

        #region SUMMARY OF BENEFITS

        /*
         * SUMMARY: Benefits of DDD approach (TREMENDOUS IMPROVEMENT!)
         * 
         * 1. ? THIN CONTROLLER: Only handles HTTP concerns (routing, status codes)
         * 2. ? BUSINESS LOGIC IN DOMAIN: Single source of truth
         * 3. ? TYPE SAFETY: Value objects prevent invalid data at compile time
         * 4. ? IMPOSSIBLE TO CREATE INVALID STATE: Domain enforces all invariants
         * 5. ? EASY TO TEST: Domain tests don't need HTTP/database
         * 6. ? NO CODE DUPLICATION: Rules enforced in one place
         * 7. ? SELF-DOCUMENTING: Domain model reads like business requirements
         * 8. ? MAINTAINABLE: Changes happen in domain, not scattered
         * 9. ? SIDE EFFECTS DECOUPLED: Domain events enable loose coupling
         * 10. ? AUDIT TRAIL: Events provide complete history
         * 
         * COMPARISON TO BadOrderController:
         * 
         * Lines of Code:
         * - BadOrderController: ~400 lines with business logic mixed in
         * - GoodOrderController: ~350 lines but ONLY HTTP concerns
         * - Order Aggregate: ~200 lines with ALL business logic
         * - Value Objects: ~300 lines total (reusable!)
         * 
         * Testability:
         * - Bad: Need HTTP test client, mock database, mock services
         * - Good: Domain tests are simple unit tests (no infrastructure)
         * 
         * Maintainability:
         * - Bad: Change requires updating multiple controllers/services
         * - Good: Change happens in domain (one place)
         * 
         * Safety:
         * - Bad: Easy to create invalid states (no protection)
         * - Good: Impossible to create invalid states (compiler + domain)
         * 
         * Performance:
         * - Bad: Same (no difference in runtime)
         * - Good: Same (no difference in runtime)
         * 
         * Learning Curve:
         * - Bad: Easy to start, hard to maintain as complexity grows
         * - Good: Requires understanding DDD, but pays off quickly
         * 
         * CONCLUSION:
         * DDD principles provide MASSIVE benefits even in layered architecture!
         * You don't need hexagonal/clean architecture to get these benefits.
         * Value Objects + Aggregates work in ANY architecture.
         */

        #endregion

        #region Request Models

        public class CreateOrderRequest
        {
            public string CustomerName { get; set; } = string.Empty;
        }

        public class AddOrderLineRequest
        {
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class CancelOrderRequest
        {
            public string Reason { get; set; } = "No reason provided";
        }

        #endregion
    }
}

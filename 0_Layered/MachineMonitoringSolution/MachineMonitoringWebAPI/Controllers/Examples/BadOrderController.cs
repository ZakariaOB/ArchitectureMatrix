using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineMonitoring.WebAPI.Controllers.Examples
{
    /// <summary>
    /// BAD EXAMPLE: Controller without DDD principles
    /// 
    /// PROBLEMS DEMONSTRATED:
    /// ? Business logic in controller (should be in domain)
    /// ? Primitive obsession (int, string, decimal everywhere)
    /// ? No validation or very weak validation
    /// ? Direct database manipulation
    /// ? Easy to create invalid states
    /// ? Hard to test (need full HTTP stack)
    /// ? Code duplication across controllers
    /// ? Mixed concerns (HTTP + business logic + data access)
    /// 
    /// This is the TYPICAL approach in many layered architectures
    /// </summary>
    [ApiController]
    [Route("api/bad/[controller]")]
    public class BadOrderController : ControllerBase
    {
        // Simulating database access (in real code, this would be DbContext)
        private static readonly List<BadOrderDto> _fakeDatabase = new();

        /// <summary>
        /// BAD: Create order endpoint with scattered business logic
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateBadOrderRequest request)
        {
            // ? PROBLEM 1: Weak validation in controller
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest("Customer name is required");
            }

            // ? PROBLEM 2: Business rule scattered in controller
            if (request.CustomerName.Length > 100)
            {
                return BadRequest("Customer name too long");
            }

            // ? PROBLEM 3: Creating entity with primitives (no type safety)
            var order = new BadOrderDto
            {
                OrderId = _fakeDatabase.Count + 1, // ? Manual ID generation
                CustomerName = request.CustomerName,
                Status = "Pending", // ? Magic string! Could typo as "Pendng"
                OrderDate = DateTime.UtcNow,
                Lines = new List<BadOrderLineDto>(),
                TotalAmount = 0 // ? Will be calculated later (inconsistent)
            };

            _fakeDatabase.Add(order);

            return Ok(new { orderId = order.OrderId, message = "Order created" });
        }

        /// <summary>
        /// BAD: Add line to order with business logic in controller
        /// </summary>
        [HttpPost("{orderId}/lines")]
        public async Task<IActionResult> AddLineToOrder(int orderId, [FromBody] AddBadOrderLineRequest request)
        {
            // ? PROBLEM 4: Finding entity in controller
            var order = _fakeDatabase.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ? PROBLEM 5: Business rule check #1 duplicated everywhere
            // "Can only modify when Pending or Confirmed"
            if (order.Status != "Pending" && order.Status != "Confirmed")
            {
                return BadRequest($"Cannot add lines to order in {order.Status} status");
            }

            // ? PROBLEM 6: Weak validation
            if (string.IsNullOrWhiteSpace(request.ProductName))
            {
                return BadRequest("Product name is required");
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be positive");
            }

            if (request.UnitPrice < 0)
            {
                return BadRequest("Price cannot be negative");
            }

            // ? PROBLEM 7: Business logic in controller
            // Check if product already exists
            var existingLine = order.Lines.FirstOrDefault(l => l.ProductName == request.ProductName);
            if (existingLine != null)
            {
                // Update quantity
                existingLine.Quantity += request.Quantity;
                existingLine.TotalPrice = existingLine.Quantity * existingLine.UnitPrice;
            }
            else
            {
                // Add new line
                var line = new BadOrderLineDto
                {
                    ProductName = request.ProductName,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalPrice = request.Quantity * request.UnitPrice // ? Manual calculation
                };
                order.Lines.Add(line);
            }

            // ? PROBLEM 8: Manual total calculation (error-prone)
            order.TotalAmount = order.Lines.Sum(l => l.TotalPrice);

            return Ok(new { message = "Line added successfully" });
        }

        /// <summary>
        /// BAD: Confirm order with business rules scattered
        /// </summary>
        [HttpPost("{orderId}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            // ? PROBLEM 9: Repeated find logic
            var order = _fakeDatabase.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ? PROBLEM 10: Business rule check duplicated
            if (order.Status != "Pending")
            {
                return BadRequest($"Can only confirm pending orders. Current status: {order.Status}");
            }

            // ? PROBLEM 11: Business rule in controller (should be in domain)
            if (!order.Lines.Any())
            {
                return BadRequest("Cannot confirm order without lines");
            }

            // ? PROBLEM 12: Business rule in controller
            if (order.TotalAmount <= 0)
            {
                return BadRequest("Total amount must be positive");
            }

            // ? PROBLEM 13: Direct state manipulation
            order.Status = "Confirmed"; // ? Magic string again!
            order.ConfirmedDate = DateTime.UtcNow;

            // ? PROBLEM 14: Side effects mixed with business logic
            // In real app, this would be:
            // await _emailService.SendConfirmationEmail(order);
            // await _inventoryService.ReserveItems(order);
            // await _auditLog.Log($"Order {orderId} confirmed");

            return Ok(new { message = "Order confirmed successfully" });
        }

        /// <summary>
        /// BAD: Cancel order with no state transition validation
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromBody] CancelBadOrderRequest request)
        {
            var order = _fakeDatabase.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ? PROBLEM 15: Incomplete business rule
            // Missing check: Can we cancel from current status?
            if (order.Status == "Completed")
            {
                return BadRequest("Cannot cancel completed order");
            }

            // ? PROBLEM 16: What about Cancelled status? Can we cancel twice?
            // This check is missing! Bug!

            // ? PROBLEM 17: Direct state change without validation
            order.Status = "Cancelled"; // ? Could be "Canceled" (typo) in another endpoint!
            order.CancellationReason = request.Reason;

            return Ok(new { message = "Order cancelled" });
        }

        /// <summary>
        /// BAD: Calculate total endpoint (business logic exposed as endpoint)
        /// </summary>
        [HttpGet("{orderId}/total")]
        public async Task<IActionResult> GetOrderTotal(int orderId)
        {
            var order = _fakeDatabase.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ? PROBLEM 18: Calculation logic in controller (should be in domain)
            var total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);

            // ? PROBLEM 19: What if stored total != calculated total?
            // No consistency guarantee!
            if (total != order.TotalAmount)
            {
                // Data corruption! But we don't know when it happened
                return StatusCode(500, "Data inconsistency detected");
            }

            return Ok(new { total });
        }

        /// <summary>
        /// BAD: Apply discount - demonstrates how business rules get more complex
        /// </summary>
        [HttpPost("{orderId}/discount")]
        public async Task<IActionResult> ApplyDiscount(int orderId, [FromBody] ApplyDiscountRequest request)
        {
            var order = _fakeDatabase.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // ? PROBLEM 20: Complex business rules in controller
            if (request.DiscountPercentage < 0 || request.DiscountPercentage > 100)
            {
                return BadRequest("Invalid discount percentage");
            }

            // ? PROBLEM 21: Who can apply discount? No authorization logic!
            // In real app: if (!User.IsInRole("Manager")) return Forbidden();

            // ? PROBLEM 22: Can we discount in any status? Missing validation!
            if (order.Status == "Completed" || order.Status == "Cancelled")
            {
                return BadRequest("Cannot apply discount to completed or cancelled order");
            }

            // ? PROBLEM 23: Manual calculation (error-prone)
            var discountAmount = order.TotalAmount * (request.DiscountPercentage / 100);
            order.TotalAmount -= discountAmount;

            // ? PROBLEM 24: What if total becomes negative? No check!
            // ? PROBLEM 25: Do we update line prices? Not clear!

            return Ok(new { newTotal = order.TotalAmount, discountApplied = discountAmount });
        }

        #region SUMMARY OF PROBLEMS

        /*
         * SUMMARY: Problems with this approach (VERY COMMON in layered architecture)
         * 
         * 1. ? BUSINESS LOGIC SCATTERED: Rules spread across multiple controllers/services
         * 2. ? PRIMITIVE OBSESSION: Using int, string, decimal everywhere (no type safety)
         * 3. ? MAGIC STRINGS: "Pending", "Confirmed", "Cancelled" (typo-prone)
         * 4. ? ANEMIC DOMAIN: DTOs are just data bags (no behavior)
         * 5. ? CODE DUPLICATION: Same validation/logic repeated everywhere
         * 6. ? EASY TO CREATE INVALID STATE: No protection against bad data
         * 7. ? HARD TO TEST: Need full HTTP stack to test business rules
         * 8. ? INCONSISTENT: Different developers implement differently
         * 9. ? DIFFICULT TO MAINTAIN: Changes require updating many files
         * 10. ? NO ENCAPSULATION: State can be changed directly
         * 11. ? SIDE EFFECTS MIXED: Email, inventory, logging all in controller
         * 12. ? NO AUDIT TRAIL: Can't track what changed and why
         * 
         * IMPACT:
         * - Bugs are common (missing validation, wrong state transitions)
         * - Development is slow (have to find and update many places)
         * - Testing is hard (need full infrastructure)
         * - Onboarding is difficult (rules not obvious)
         * - Refactoring is risky (might break something)
         * 
         * SOLUTION: Use DDD principles (see GoodOrderController)
         */

        #endregion

        #region DTOs and Request Models

        public class BadOrderDto
        {
            public int OrderId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public DateTime? ConfirmedDate { get; set; }
            public DateTime? CompletedDate { get; set; }
            public string? CancellationReason { get; set; }
            public decimal TotalAmount { get; set; }
            public List<BadOrderLineDto> Lines { get; set; } = new();
        }

        public class BadOrderLineDto
        {
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class CreateBadOrderRequest
        {
            public string CustomerName { get; set; } = string.Empty;
        }

        public class AddBadOrderLineRequest
        {
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class CancelBadOrderRequest
        {
            public string Reason { get; set; } = string.Empty;
        }

        public class ApplyDiscountRequest
        {
            public decimal DiscountPercentage { get; set; }
        }

        #endregion
    }
}

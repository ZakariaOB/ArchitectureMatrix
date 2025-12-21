using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineMonitoringRepository.Models.DDD
{
    /// <summary>
    /// Order Aggregate Root
    /// 
    /// DDD Principles Demonstrated:
    /// 1. Aggregate Root - Order is the consistency boundary
    /// 2. Encapsulation - Private setters, behavior through methods
    /// 3. Domain Logic - Business rules enforced in the entity
    /// 4. Invariants - Order is always in a valid state
    /// 5. Domain Events - Significant changes raise events
    /// 6. Value Objects - Uses OrderId, OrderStatus, OrderLine
    /// 
    /// WHY THIS IMPROVES LAYERED ARCHITECTURE:
    /// - Business rules are NOT scattered in controllers/services
    /// - Domain logic is testable without database
    /// - Impossible to create invalid orders
    /// - Changes are auditable through events
    /// </summary>
    public class Order
    {
        private readonly List<OrderLine> _lines = new List<OrderLine>();
        private readonly List<object> _domainEvents = new List<object>();

        public OrderId Id { get; private set; }
        public string CustomerName { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }

        // Expose lines as read-only
        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        // Required by EF Core
        private Order() 
        {
            Id = OrderId.Empty();
            CustomerName = string.Empty;
            Status = OrderStatus.Pending;
        }

        // Factory method - ensures valid creation
        private Order(string customerName, DateTime orderDate)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required", nameof(customerName));

            if (customerName.Length > 100)
                throw new ArgumentException("Customer name too long", nameof(customerName));

            Id = OrderId.Empty(); // Will be set by database
            CustomerName = customerName;
            Status = OrderStatus.Pending;
            OrderDate = orderDate;
        }

        /// <summary>
        /// DDD: Public factory method to create new orders
        /// </summary>
        public static Order Create(string customerName)
        {
            return new Order(customerName, DateTime.UtcNow);
        }

        /// <summary>
        /// DDD Business Rule: Add line to order
        /// Can only add lines when order is modifiable
        /// </summary>
        public void AddLine(OrderLine line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            if (!Status.CanModifyOrder())
                throw new InvalidOperationException(
                    $"Cannot add lines to order in {Status} status");

            // DDD: Check if line already exists (same product)
            var existingLine = _lines.FirstOrDefault(l => l.ProductName == line.ProductName);
            if (existingLine != null)
            {
                // Update quantity instead of adding duplicate
                _lines.Remove(existingLine);
                _lines.Add(existingLine.WithQuantity(existingLine.Quantity + line.Quantity));
            }
            else
            {
                _lines.Add(line);
            }
        }

        /// <summary>
        /// DDD Business Rule: Remove line from order
        /// </summary>
        public void RemoveLine(string productName)
        {
            if (!Status.CanModifyOrder())
                throw new InvalidOperationException(
                    $"Cannot remove lines from order in {Status} status");

            var line = _lines.FirstOrDefault(l => l.ProductName == productName);
            if (line != null)
            {
                _lines.Remove(line);
            }
        }

        /// <summary>
        /// DDD Business Rule: Confirm order
        /// Order must have at least one line
        /// </summary>
        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException(
                    $"Cannot confirm order in {Status} status");

            if (!_lines.Any())
                throw new InvalidOperationException(
                    "Cannot confirm order without any lines");

            if (GetTotalAmount() <= 0)
                throw new InvalidOperationException(
                    "Cannot confirm order with zero or negative total");

            ChangeStatus(OrderStatus.Confirmed);

            // Raise domain event
            _domainEvents.Add(new OrderPlacedEvent(
                Id,
                DateTime.UtcNow,
                GetTotalAmount(),
                CustomerName));
        }

        /// <summary>
        /// DDD Business Rule: Start production
        /// </summary>
        public void StartProduction()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException(
                    $"Cannot start production for order in {Status} status");

            ChangeStatus(OrderStatus.InProduction);
        }

        /// <summary>
        /// DDD Business Rule: Complete order
        /// </summary>
        public void Complete()
        {
            if (Status != OrderStatus.InProduction)
                throw new InvalidOperationException(
                    $"Cannot complete order in {Status} status");

            ChangeStatus(OrderStatus.Completed);
            CompletedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// DDD Business Rule: Cancel order
        /// </summary>
        public void Cancel(string reason)
        {
            if (!Status.CanBeCancelled())
                throw new InvalidOperationException(
                    $"Cannot cancel order in {Status} status");

            var oldStatus = Status;
            ChangeStatus(OrderStatus.Cancelled);

            // Raise domain event
            _domainEvents.Add(new OrderCancelledEvent(
                Id,
                DateTime.UtcNow,
                reason));
        }

        /// <summary>
        /// DDD: Change status with validation
        /// </summary>
        private void ChangeStatus(OrderStatus newStatus)
        {
            if (!Status.CanTransitionTo(newStatus))
                throw new InvalidOperationException(
                    $"Invalid status transition from {Status} to {newStatus}");

            var oldStatus = Status;
            Status = newStatus;

            // Raise domain event
            _domainEvents.Add(new OrderStatusChangedEvent(
                Id,
                oldStatus,
                newStatus,
                DateTime.UtcNow));
        }

        /// <summary>
        /// DDD: Calculate total amount
        /// </summary>
        public decimal GetTotalAmount()
        {
            return _lines.Sum(line => line.TotalPrice);
        }

        /// <summary>
        /// DDD: Get number of items
        /// </summary>
        public int GetTotalItems()
        {
            return _lines.Sum(line => line.Quantity);
        }

        /// <summary>
        /// DDD Business Rule: Check if order meets minimum value
        /// </summary>
        public bool MeetsMinimumOrderValue(decimal minimumValue)
        {
            return GetTotalAmount() >= minimumValue;
        }

        /// <summary>
        /// DDD: Check if order has specific product
        /// </summary>
        public bool HasProduct(string productName)
        {
            return _lines.Any(l => l.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Clear domain events after publishing
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public override string ToString()
        {
            return $"Order {Id} - {CustomerName} - {Status} - {Lines.Count} lines - ${GetTotalAmount():F2}";
        }
    }
}

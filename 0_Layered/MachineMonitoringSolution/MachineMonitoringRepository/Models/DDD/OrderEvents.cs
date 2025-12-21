using System;

namespace MachineMonitoringRepository.Models.DDD
{
    /// <summary>
    /// Domain Event for Order Placed
    /// 
    /// DDD Principle: Domain Events capture significant business occurrences.
    /// 
    /// Benefits:
    /// - Loose coupling between aggregates
    /// - Audit trail of business events
    /// - Enable asynchronous processing
    /// - Support event sourcing if needed
    /// </summary>
    public sealed class OrderPlacedEvent
    {
        public OrderId OrderId { get; }
        public DateTime OccurredAt { get; }
        public decimal TotalAmount { get; }
        public string CustomerName { get; }

        public OrderPlacedEvent(OrderId orderId, DateTime occurredAt, decimal totalAmount, string customerName)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OccurredAt = occurredAt;
            TotalAmount = totalAmount;
            CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        }

        public override string ToString() =>
            $"Order {OrderId} placed at {OccurredAt:yyyy-MM-dd HH:mm:ss} for ${TotalAmount:F2} by {CustomerName}";
    }

    /// <summary>
    /// Domain Event for Order Status Changed
    /// </summary>
    public sealed class OrderStatusChangedEvent
    {
        public OrderId OrderId { get; }
        public OrderStatus OldStatus { get; }
        public OrderStatus NewStatus { get; }
        public DateTime OccurredAt { get; }

        public OrderStatusChangedEvent(OrderId orderId, OrderStatus oldStatus, OrderStatus newStatus, DateTime occurredAt)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OldStatus = oldStatus ?? throw new ArgumentNullException(nameof(oldStatus));
            NewStatus = newStatus ?? throw new ArgumentNullException(nameof(newStatus));
            OccurredAt = occurredAt;
        }

        public override string ToString() =>
            $"Order {OrderId} status changed from {OldStatus} to {NewStatus} at {OccurredAt:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// Domain Event for Order Cancelled
    /// </summary>
    public sealed class OrderCancelledEvent
    {
        public OrderId OrderId { get; }
        public DateTime OccurredAt { get; }
        public string Reason { get; }

        public OrderCancelledEvent(OrderId orderId, DateTime occurredAt, string reason)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            OccurredAt = occurredAt;
            Reason = reason ?? "No reason provided";
        }

        public override string ToString() =>
            $"Order {OrderId} cancelled at {OccurredAt:yyyy-MM-dd HH:mm:ss}. Reason: {Reason}";
    }
}

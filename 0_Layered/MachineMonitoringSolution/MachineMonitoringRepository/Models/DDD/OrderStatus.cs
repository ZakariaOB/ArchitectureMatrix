using System;
using System.Collections.Generic;
using System.Linq;

namespace MachineMonitoringRepository.Models.DDD
{
    /// <summary>
    /// Value Object for Order Status
    /// 
    /// DDD Principle: Use rich types instead of primitives or simple enums.
    /// This encapsulates status-related business rules.
    /// 
    /// Benefits:
    /// - Business logic lives with the data
    /// - Valid transitions are explicit
    /// - Impossible to create invalid states
    /// </summary>
    public sealed class OrderStatus : IEquatable<OrderStatus>
    {
        public string Value { get; }

        // Valid statuses
        public static readonly OrderStatus Pending = new OrderStatus("Pending");
        public static readonly OrderStatus Confirmed = new OrderStatus("Confirmed");
        public static readonly OrderStatus InProduction = new OrderStatus("InProduction");
        public static readonly OrderStatus Completed = new OrderStatus("Completed");
        public static readonly OrderStatus Cancelled = new OrderStatus("Cancelled");

        private OrderStatus(string value)
        {
            Value = value;
        }

        public static OrderStatus FromString(string value)
        {
            return GetAll().FirstOrDefault(s => s.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                   ?? throw new ArgumentException($"Invalid order status: {value}", nameof(value));
        }

        public static IEnumerable<OrderStatus> GetAll()
        {
            yield return Pending;
            yield return Confirmed;
            yield return InProduction;
            yield return Completed;
            yield return Cancelled;
        }

        /// <summary>
        /// DDD Business Rule: Valid status transitions
        /// </summary>
        public bool CanTransitionTo(OrderStatus newStatus)
        {
            if (this == Pending)
                return newStatus == Confirmed || newStatus == Cancelled;

            if (this == Confirmed)
                return newStatus == InProduction || newStatus == Cancelled;

            if (this == InProduction)
                return newStatus == Completed || newStatus == Cancelled;

            if (this == Completed || this == Cancelled)
                return false; // Terminal states

            return false;
        }

        /// <summary>
        /// DDD Business Rule: Can order be modified?
        /// </summary>
        public bool CanModifyOrder() => this == Pending || this == Confirmed;

        /// <summary>
        /// DDD Business Rule: Can order be cancelled?
        /// </summary>
        public bool CanBeCancelled() => this != Completed && this != Cancelled;

        public bool Equals(OrderStatus? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => obj is OrderStatus other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;

        public static bool operator ==(OrderStatus? left, OrderStatus? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(OrderStatus? left, OrderStatus? right) => !(left == right);

        // For EF Core
        public static implicit operator string(OrderStatus status) => status.Value;
    }
}

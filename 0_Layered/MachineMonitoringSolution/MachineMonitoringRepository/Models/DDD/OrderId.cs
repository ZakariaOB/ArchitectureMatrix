using System;

namespace MachineMonitoringRepository.Models.DDD
{
    /// <summary>
    /// Value Object for Order ID
    /// 
    /// DDD Principle: Value Objects encapsulate primitive types and add domain meaning.
    /// Benefits:
    /// - Type safety: Can't accidentally pass int where OrderId is expected
    /// - Validation: Business rules enforced at construction
    /// - Immutability: Once created, cannot be changed
    /// - Self-documenting: OrderId is more meaningful than int
    /// </summary>
    public sealed class OrderId : IEquatable<OrderId>
    {
        public int Value { get; }

        private OrderId(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method with validation
        /// </summary>
        public static OrderId Create(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Order ID must be positive", nameof(value));

            return new OrderId(value);
        }

        /// <summary>
        /// For new orders that don't have ID yet
        /// </summary>
        public static OrderId Empty() => new OrderId(0);

        public bool IsEmpty() => Value == 0;

        // Value Object equality
        public bool Equals(OrderId? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => obj is OrderId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"OrderId({Value})";

        public static bool operator ==(OrderId? left, OrderId? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(OrderId? left, OrderId? right) => !(left == right);

        // Conversion for EF Core
        public static implicit operator int(OrderId orderId) => orderId.Value;
    }
}

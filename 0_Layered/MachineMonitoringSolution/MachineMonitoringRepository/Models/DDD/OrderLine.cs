using System;

namespace MachineMonitoringRepository.Models.DDD
{
    /// <summary>
    /// Value Object for Order Line
    /// 
    /// DDD Principle: Entities vs Value Objects
    /// - Entities have identity and lifecycle
    /// - Value Objects are defined by their attributes
    /// 
    /// OrderLine is a Value Object because:
    /// - Two order lines with same product/quantity are interchangeable
    /// - No need to track individual line changes
    /// - Immutable - replace the whole object if changes needed
    /// </summary>
    public sealed class OrderLine : IEquatable<OrderLine>
    {
        public string ProductName { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public decimal TotalPrice => Quantity * UnitPrice;

        private OrderLine(string productName, int quantity, decimal unitPrice)
        {
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        /// <summary>
        /// Factory method with business rule validation
        /// </summary>
        public static OrderLine Create(string productName, int quantity, decimal unitPrice)
        {
            // DDD Business Rules
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name is required", nameof(productName));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (productName.Length > 200)
                throw new ArgumentException("Product name too long", nameof(productName));

            return new OrderLine(productName, quantity, unitPrice);
        }

        /// <summary>
        /// DDD: Create new OrderLine with updated quantity (immutability)
        /// </summary>
        public OrderLine WithQuantity(int newQuantity)
        {
            return Create(ProductName, newQuantity, UnitPrice);
        }

        /// <summary>
        /// DDD Business Rule: Minimum order value
        /// </summary>
        public bool MeetsMinimumOrderValue(decimal minimumValue)
        {
            return TotalPrice >= minimumValue;
        }

        // Value Object equality - based on all properties
        public bool Equals(OrderLine? other)
        {
            if (other is null) return false;
            return ProductName == other.ProductName
                   && Quantity == other.Quantity
                   && UnitPrice == other.UnitPrice;
        }

        public override bool Equals(object? obj) => obj is OrderLine other && Equals(other);

        public override int GetHashCode()
        {
            return HashCode.Combine(ProductName, Quantity, UnitPrice);
        }

        public override string ToString() => $"{ProductName} x {Quantity} @ ${UnitPrice:F2} = ${TotalPrice:F2}";

        public static bool operator ==(OrderLine? left, OrderLine? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(OrderLine? left, OrderLine? right) => !(left == right);
    }
}

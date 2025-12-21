using MachineMonitoringRepository.Models.DDD;
using Xunit;
using System;

namespace MachineMonitoring.Tests.DDD
{
    /// <summary>
    /// Tests for Value Objects
    /// 
    /// DEMONSTRATES:
    /// - Value Objects enforce business rules at construction
    /// - Immutability prevents accidental changes
    /// - Equality based on value, not reference
    /// - Type safety prevents primitive obsession
    /// </summary>
    public class ValueObjectTests
    {
        #region OrderId Tests

        [Fact]
        public void OrderId_Create_WithValidId_CreatesOrderId()
        {
            // Act
            var orderId = OrderId.Create(123);

            // Assert
            Assert.Equal(123, orderId.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void OrderId_Create_WithInvalidId_ThrowsException(int value)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => OrderId.Create(value));
        }

        [Fact]
        public void OrderId_Empty_CreatesEmptyId()
        {
            // Act
            var orderId = OrderId.Empty();

            // Assert
            Assert.True(orderId.IsEmpty());
            Assert.Equal(0, orderId.Value);
        }

        [Fact]
        public void OrderId_Equality_BasedOnValue()
        {
            // Arrange
            var id1 = OrderId.Create(123);
            var id2 = OrderId.Create(123);
            var id3 = OrderId.Create(456);

            // Assert
            Assert.Equal(id1, id2);
            Assert.True(id1 == id2);
            Assert.NotEqual(id1, id3);
            Assert.True(id1 != id3);
        }

        #endregion

        #region OrderLine Tests

        [Fact]
        public void OrderLine_Create_WithValidData_CreatesOrderLine()
        {
            // Act
            var line = OrderLine.Create("Widget", 5, 10.00m);

            // Assert
            Assert.Equal("Widget", line.ProductName);
            Assert.Equal(5, line.Quantity);
            Assert.Equal(10.00m, line.UnitPrice);
            Assert.Equal(50.00m, line.TotalPrice);
        }

        [Theory]
        [InlineData(null, 5, 10.00)]
        [InlineData("", 5, 10.00)]
        [InlineData("   ", 5, 10.00)]
        public void OrderLine_Create_WithInvalidProductName_ThrowsException(string productName, int quantity, decimal price)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => OrderLine.Create(productName, quantity, price));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void OrderLine_Create_WithInvalidQuantity_ThrowsException(int quantity)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                OrderLine.Create("Widget", quantity, 10.00m));
            Assert.Contains("positive", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void OrderLine_Create_WithNegativePrice_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                OrderLine.Create("Widget", 5, -10.00m));
            Assert.Contains("cannot be negative", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void OrderLine_Create_WithTooLongProductName_ThrowsException()
        {
            // Arrange
            string longName = new string('A', 201);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => OrderLine.Create(longName, 1, 10.00m));
        }

        [Fact]
        public void OrderLine_WithQuantity_CreatesNewImmutableInstance()
        {
            // Arrange
            var original = OrderLine.Create("Widget", 5, 10.00m);

            // Act
            var modified = original.WithQuantity(10);

            // Assert
            Assert.Equal(5, original.Quantity); // Original unchanged
            Assert.Equal(10, modified.Quantity); // New instance
            Assert.Equal("Widget", modified.ProductName); // Same product
            Assert.Equal(10.00m, modified.UnitPrice); // Same price
            Assert.NotSame(original, modified); // Different instances
        }

        [Fact]
        public void OrderLine_Equality_BasedOnAllProperties()
        {
            // Arrange
            var line1 = OrderLine.Create("Widget", 5, 10.00m);
            var line2 = OrderLine.Create("Widget", 5, 10.00m);
            var line3 = OrderLine.Create("Widget", 3, 10.00m); // Different quantity
            var line4 = OrderLine.Create("Gadget", 5, 10.00m); // Different product

            // Assert
            Assert.Equal(line1, line2);
            Assert.True(line1 == line2);
            Assert.NotEqual(line1, line3);
            Assert.NotEqual(line1, line4);
        }

        [Fact]
        public void OrderLine_TotalPrice_CalculatesCorrectly()
        {
            // Arrange & Act
            var line = OrderLine.Create("Widget", 7, 12.50m);

            // Assert
            Assert.Equal(87.50m, line.TotalPrice); // 7 * 12.50
        }

        [Theory]
        [InlineData(50.00, true)]
        [InlineData(50.01, false)]
        [InlineData(49.99, true)]
        public void OrderLine_MeetsMinimumOrderValue_ReturnsCorrectResult(decimal minimum, bool expected)
        {
            // Arrange
            var line = OrderLine.Create("Widget", 5, 10.00m); // Total: 50.00

            // Act
            var result = line.MeetsMinimumOrderValue(minimum);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region OrderStatus Tests

        [Fact]
        public void OrderStatus_FromString_WithValidStatus_ReturnsCorrectStatus()
        {
            // Act
            var pending = OrderStatus.FromString("Pending");
            var confirmed = OrderStatus.FromString("confirmed"); // Case insensitive

            // Assert
            Assert.Equal(OrderStatus.Pending, pending);
            Assert.Equal(OrderStatus.Confirmed, confirmed);
        }

        [Fact]
        public void OrderStatus_FromString_WithInvalidStatus_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => OrderStatus.FromString("InvalidStatus"));
        }

        [Fact]
        public void OrderStatus_CanTransitionTo_EnforcesValidTransitions()
        {
            // Arrange & Assert
            // From Pending
            Assert.True(OrderStatus.Pending.CanTransitionTo(OrderStatus.Confirmed));
            Assert.True(OrderStatus.Pending.CanTransitionTo(OrderStatus.Cancelled));
            Assert.False(OrderStatus.Pending.CanTransitionTo(OrderStatus.InProduction));
            Assert.False(OrderStatus.Pending.CanTransitionTo(OrderStatus.Completed));

            // From Confirmed
            Assert.True(OrderStatus.Confirmed.CanTransitionTo(OrderStatus.InProduction));
            Assert.True(OrderStatus.Confirmed.CanTransitionTo(OrderStatus.Cancelled));
            Assert.False(OrderStatus.Confirmed.CanTransitionTo(OrderStatus.Completed));

            // From InProduction
            Assert.True(OrderStatus.InProduction.CanTransitionTo(OrderStatus.Completed));
            Assert.True(OrderStatus.InProduction.CanTransitionTo(OrderStatus.Cancelled));
            Assert.False(OrderStatus.InProduction.CanTransitionTo(OrderStatus.Pending));

            // From Completed (terminal)
            Assert.False(OrderStatus.Completed.CanTransitionTo(OrderStatus.Cancelled));
            Assert.False(OrderStatus.Completed.CanTransitionTo(OrderStatus.Pending));

            // From Cancelled (terminal)
            Assert.False(OrderStatus.Cancelled.CanTransitionTo(OrderStatus.Pending));
            Assert.False(OrderStatus.Cancelled.CanTransitionTo(OrderStatus.Confirmed));
        }

        [Theory]
        [InlineData("Pending", true)]
        [InlineData("Confirmed", true)]
        [InlineData("InProduction", false)]
        [InlineData("Completed", false)]
        [InlineData("Cancelled", false)]
        public void OrderStatus_CanModifyOrder_ReturnsCorrectResult(string statusName, bool expected)
        {
            // Arrange
            var status = OrderStatus.FromString(statusName);

            // Act & Assert
            Assert.Equal(expected, status.CanModifyOrder());
        }

        [Theory]
        [InlineData("Pending", true)]
        [InlineData("Confirmed", true)]
        [InlineData("InProduction", true)]
        [InlineData("Completed", false)]
        [InlineData("Cancelled", false)]
        public void OrderStatus_CanBeCancelled_ReturnsCorrectResult(string statusName, bool expected)
        {
            // Arrange
            var status = OrderStatus.FromString(statusName);

            // Act & Assert
            Assert.Equal(expected, status.CanBeCancelled());
        }

        [Fact]
        public void OrderStatus_GetAll_ReturnsAllStatuses()
        {
            // Act
            var allStatuses = OrderStatus.GetAll();

            // Assert
            Assert.Contains(OrderStatus.Pending, allStatuses);
            Assert.Contains(OrderStatus.Confirmed, allStatuses);
            Assert.Contains(OrderStatus.InProduction, allStatuses);
            Assert.Contains(OrderStatus.Completed, allStatuses);
            Assert.Contains(OrderStatus.Cancelled, allStatuses);
            Assert.Equal(5, allStatuses.Count());
        }

        [Fact]
        public void OrderStatus_Equality_WorksCorrectly()
        {
            // Arrange
            var status1 = OrderStatus.Pending;
            var status2 = OrderStatus.FromString("Pending");
            var status3 = OrderStatus.Confirmed;

            // Assert
            Assert.Equal(status1, status2);
            Assert.True(status1 == status2);
            Assert.NotEqual(status1, status3);
            Assert.True(status1 != status3);
        }

        #endregion
    }
}

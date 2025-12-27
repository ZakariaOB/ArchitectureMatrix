using MachineMonitoringRepository.Models.DDD;

namespace MachineMonitoring.Tests.DDD
{
    /// <summary>
    /// Unit tests for Order DDD aggregate
    /// 
    /// DEMONSTRATES:
    /// - Testing domain logic WITHOUT database
    /// - Testing domain logic WITHOUT services/controllers
    /// - Business rules are EXPLICIT and TESTABLE
    /// - Fast tests (no I/O)
    /// 
    /// This is a KEY benefit of DDD in ANY architecture!
    /// </summary>
    public class OrderTests
    {
        #region Creation Tests

        [Fact]
        public void Order_Create_WithValidCustomer_CreatesOrder()
        {
            // Arrange & Act
            var order = Order.Create("John Doe");

            // Assert
            Assert.NotNull(order);
            Assert.Equal("John Doe", order.CustomerName);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.NotEqual(default(DateTime), order.OrderDate);
            Assert.Empty(order.Lines);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Order_Create_WithInvalidCustomer_ThrowsException(string customerName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Order.Create(customerName));
        }

        [Fact]
        public void Order_Create_WithTooLongCustomerName_ThrowsException()
        {
            // Arrange
            string longName = new string('A', 101);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Order.Create(longName));
            Assert.Contains("too long", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Add Line Tests

        [Fact]
        public void Order_AddLine_ToPendingOrder_AddsLine()
        {
            // Arrange
            var order = Order.Create("John Doe");
            var line = OrderLine.Create("Widget", 5, 10.00m);

            // Act
            order.AddLine(line);

            // Assert
            Assert.Single(order.Lines);
            Assert.Contains(order.Lines, l => l.ProductName == "Widget");
            Assert.Equal(50.00m, order.GetTotalAmount());
        }

        [Fact]
        public void Order_AddLine_SameProductTwice_CombinesQuantity()
        {
            // Arrange
            var order = Order.Create("John Doe");
            var line1 = OrderLine.Create("Widget", 5, 10.00m);
            var line2 = OrderLine.Create("Widget", 3, 10.00m);

            // Act
            order.AddLine(line1);
            order.AddLine(line2);

            // Assert
            Assert.Single(order.Lines);
            var resultLine = order.Lines.First();
            Assert.Equal(8, resultLine.Quantity); // 5 + 3
            Assert.Equal(80.00m, order.GetTotalAmount()); // 8 * 10
        }

        [Fact]
        public void Order_AddLine_ToCompletedOrder_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();
            order.StartProduction();
            order.Complete();

            var newLine = OrderLine.Create("Gadget", 1, 20.00m);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => order.AddLine(newLine));
            Assert.Contains("Cannot add lines", exception.Message);
            Assert.Contains("Completed", exception.Message);
        }

        #endregion

        #region Confirm Tests

        [Fact]
        public void Order_Confirm_WithLines_ConfirmsOrder()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));

            // Act
            order.Confirm();

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            
            // Check domain event was raised
            var events = order.DomainEvents.ToList();
            Assert.Single(events);
            var orderPlacedEvent = Assert.IsType<OrderPlacedEvent>(events[0]);
            Assert.Equal(50.00m, orderPlacedEvent.TotalAmount);
            Assert.Equal("John Doe", orderPlacedEvent.CustomerName);
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

        [Fact]
        public void Order_Confirm_WhenAlreadyConfirmed_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.Confirm());
        }

        #endregion

        #region Status Transition Tests

        [Fact]
        public void Order_StatusTransitions_FollowValidFlow()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));

            // Act & Assert - Valid flow
            Assert.Equal(OrderStatus.Pending, order.Status);

            order.Confirm();
            Assert.Equal(OrderStatus.Confirmed, order.Status);

            order.StartProduction();
            Assert.Equal(OrderStatus.InProduction, order.Status);

            order.Complete();
            Assert.Equal(OrderStatus.Completed, order.Status);
            Assert.NotNull(order.CompletedDate);
        }

        [Fact]
        public void Order_StartProduction_WhenNotConfirmed_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => order.StartProduction());
            Assert.Contains("Confirmed", exception.Message);
        }

        [Fact]
        public void Order_Complete_WhenNotInProduction_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => order.Complete());
            Assert.Contains("InProduction", exception.Message);
        }

        #endregion

        #region Cancel Tests

        [Fact]
        public void Order_Cancel_WhenPending_CancelsOrder()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));

            // Act
            order.Cancel("Customer requested cancellation");

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            
            // Check domain event
            var events = order.DomainEvents.ToList();
            var cancelEvent = events.OfType<OrderCancelledEvent>().FirstOrDefault();
            Assert.NotNull(cancelEvent);
            Assert.Contains("Customer requested", cancelEvent.Reason);
        }

        [Fact]
        public void Order_Cancel_WhenCompleted_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();
            order.StartProduction();
            order.Complete();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => order.Cancel("Too late"));
            Assert.Contains("Cannot cancel", exception.Message);
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public void Order_GetTotalAmount_ReturnsCorrectSum()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));   // 50.00
            order.AddLine(OrderLine.Create("Gadget", 3, 15.00m));   // 45.00

            // Act
            var total = order.GetTotalAmount();

            // Assert
            Assert.Equal(95.00m, total);
        }

        [Fact]
        public void Order_GetTotalItems_ReturnsCorrectCount()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            order.AddLine(OrderLine.Create("Gadget", 3, 15.00m));

            // Act
            var totalItems = order.GetTotalItems();

            // Assert
            Assert.Equal(8, totalItems); // 5 + 3
        }

        [Theory]
        [InlineData(100.00, true)]
        [InlineData(95.00, true)]
        [InlineData(95.01, false)]
        public void Order_MeetsMinimumOrderValue_ReturnsCorrectResult(decimal minimum, bool expected)
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));   // 50.00
            order.AddLine(OrderLine.Create("Gadget", 3, 15.00m));   // 45.00
            // Total: 95.00

            // Act
            var result = order.MeetsMinimumOrderValue(minimum);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Order_HasProduct_ReturnsTrueWhenProductExists()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));

            // Act & Assert
            Assert.True(order.HasProduct("Widget"));
            Assert.True(order.HasProduct("widget")); // Case insensitive
            Assert.False(order.HasProduct("Gadget"));
        }

        #endregion

        #region Domain Events Tests

        [Fact]
        public void Order_RaisesDomainEvents_ForSignificantChanges()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));

            // Act
            order.Confirm();
            order.StartProduction();

            // Assert - Multiple events raised
            var events = order.DomainEvents.ToList();
            Assert.Equal(3, events.Count); // OrderPlaced + 2 StatusChanged

            Assert.Single(events.OfType<OrderPlacedEvent>());
            Assert.Equal(2, events.OfType<OrderStatusChangedEvent>().Count());
        }

        [Fact]
        public void Order_ClearDomainEvents_RemovesAllEvents()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();

            Assert.NotEmpty(order.DomainEvents);

            // Act
            order.ClearDomainEvents();

            // Assert
            Assert.Empty(order.DomainEvents);
        }

        #endregion

        #region Remove Line Tests

        [Fact]
        public void Order_RemoveLine_RemovesExistingLine()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 5, 10.00m));
            order.AddLine(OrderLine.Create("Gadget", 3, 15.00m));

            Assert.Equal(2, order.Lines.Count);

            // Act
            order.RemoveLine("Widget");

            // Assert
            Assert.Single(order.Lines);
            Assert.False(order.HasProduct("Widget"));
            Assert.True(order.HasProduct("Gadget"));
        }

        [Fact]
        public void Order_RemoveLine_FromCompletedOrder_ThrowsException()
        {
            // Arrange
            var order = Order.Create("John Doe");
            order.AddLine(OrderLine.Create("Widget", 1, 10.00m));
            order.Confirm();
            order.StartProduction();
            order.Complete();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => order.RemoveLine("Widget"));
        }

        #endregion
    }
}

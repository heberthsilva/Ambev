using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Xunit;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class SaleItemTests
    {
        [Theory(DisplayName = "Should apply 10% discount for quantity between 4 and 9")]
        [InlineData(4, 100, 40)] // 4 items, 100 unit price, 10% discount = 40
        [InlineData(9, 100, 90)] // 9 items, 100 unit price, 10% discount = 90
        public void CalculateDiscount_ShouldApply10PercentDiscount(int quantity, decimal unitPrice, decimal expectedDiscount)
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product A", quantity, unitPrice);

            // Act
            // Discount is calculated in the constructor via SetQuantity

            // Assert
            Assert.Equal(expectedDiscount, saleItem.Discount);
            Assert.Equal((unitPrice * quantity) - expectedDiscount, saleItem.TotalItemAmount);
        }

        [Theory(DisplayName = "Should apply 20% discount for quantity between 10 and 20")]
        [InlineData(10, 100, 200)] // 10 items, 100 unit price, 20% discount = 200
        [InlineData(20, 100, 400)] // 20 items, 100 unit price, 20% discount = 400
        public void CalculateDiscount_ShouldApply20PercentDiscount(int quantity, decimal unitPrice, decimal expectedDiscount)
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product B", quantity, unitPrice);

            // Act
            // Discount is calculated in the constructor via SetQuantity

            // Assert
            Assert.Equal(expectedDiscount, saleItem.Discount);
            Assert.Equal((unitPrice * quantity) - expectedDiscount, saleItem.TotalItemAmount);
        }

        [Theory(DisplayName = "Should not apply discount for quantity below 4")]
        [InlineData(1, 100)]
        [InlineData(3, 100)]
        public void CalculateDiscount_ShouldNotApplyDiscountForQuantityBelow4(int quantity, decimal unitPrice)
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product C", quantity, unitPrice);

            // Act
            // Discount is calculated in the constructor via SetQuantity

            // Assert
            Assert.Equal(0, saleItem.Discount);
            Assert.Equal(unitPrice * quantity, saleItem.TotalItemAmount);
        }

        [Fact(DisplayName = "Should throw DomainException when quantity is above 20")]
        public void SetQuantity_ShouldThrowDomainException_WhenQuantityIsAbove20()
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product D", 1, 100);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => saleItem.SetQuantity(21));
            Assert.Equal("Não é possível vender mais de 20 itens idênticos.", exception.Message);
        }

        [Fact(DisplayName = "Should cancel item and set total amount to zero")]
        public void CancelItem_ShouldCancelItemAndSetTotalAmountToZero()
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product E", 5, 100);
            var saleId = Guid.NewGuid();

            // Act
            saleItem.CancelItem(saleId);

            // Assert
            Assert.True(saleItem.IsCancelled);
            Assert.Equal(0, saleItem.TotalItemAmount);
            Assert.Equal(0, saleItem.Discount);
        }

        [Fact(DisplayName = "Should not cancel item if already cancelled")]
        public void CancelItem_ShouldNotCancelItemIfAlreadyCancelled()
        {
            // Arrange
            var saleItem = new SaleItem(Guid.NewGuid(), "Product F", 5, 100);
            var saleId = Guid.NewGuid();
            saleItem.CancelItem(saleId); // First cancellation

            // Store initial state
            var initialTotalAmount = saleItem.TotalItemAmount;
            var initialDiscount = saleItem.Discount;

            // Act
            saleItem.CancelItem(saleId); // Second cancellation

            // Assert
            Assert.True(saleItem.IsCancelled);
            Assert.Equal(initialTotalAmount, saleItem.TotalItemAmount); // Should remain 0
            Assert.Equal(initialDiscount, saleItem.Discount); // Should remain 0
        }
    }
}
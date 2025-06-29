using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class SaleCommandHandlerTests
    {
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<CreateSaleCommandHandler>> _mockCreateLogger;
        private readonly Mock<ILogger<UpdateSaleCommandHandler>> _mockUpdateLogger;
        private readonly Mock<ILogger<DeleteSaleCommandHandler>> _mockDeleteLogger;

        public SaleCommandHandlerTests()
        {
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockCreateLogger = new Mock<ILogger<CreateSaleCommandHandler>>();
            _mockUpdateLogger = new Mock<ILogger<UpdateSaleCommandHandler>>();
            _mockDeleteLogger = new Mock<ILogger<DeleteSaleCommandHandler>>();
        }

        [Fact(DisplayName = "CreateSaleCommandHandler should publish SaleCreatedEvent")]
        public async Task CreateSaleCommandHandler_ShouldPublishSaleCreatedEvent()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = 1,
                SaleDate = DateTime.Now,
                Customer = "Test Customer",
                Branch = "Test Branch",
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand { ProductId = Guid.NewGuid(), ProductName = "Product 1", Quantity = 5, UnitPrice = 100 }
                }
            };

            var handler = new CreateSaleCommandHandler(_mockSaleRepository.Object, _mockMediator.Object, _mockCreateLogger.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockSaleRepository.Verify(r => r.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<SaleCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "UpdateSaleCommandHandler should publish SaleModifiedEvent")]
        public async Task UpdateSaleCommandHandler_ShouldPublishSaleModifiedEvent()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale(1, DateTime.Now, "Existing Customer", "Existing Branch");
            existingSale.AddItem(new SaleItem(Guid.NewGuid(), "Product X", 1, 10));

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 2,
                SaleDate = DateTime.Now,
                Customer = "Updated Customer",
                Branch = "Updated Branch",
                IsCancelled = false,
                Items = new List<UpdateSaleItemCommand>
                {
                    new UpdateSaleItemCommand { ProductId = Guid.NewGuid(), ProductName = "Product Y", Quantity = 2, UnitPrice = 20 }
                }
            };

            var handler = new UpdateSaleCommandHandler(_mockSaleRepository.Object, _mockMediator.Object, _mockUpdateLogger.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<SaleModifiedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "UpdateSaleCommandHandler should publish SaleCancelledEvent when sale is cancelled")]
        public async Task UpdateSaleCommandHandler_ShouldPublishSaleCancelledEvent_WhenSaleIsCancelled()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale(1, DateTime.Now, "Existing Customer", "Existing Branch");

            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSale);

            var command = new UpdateSaleCommand
            {
                Id = saleId,
                SaleNumber = 1,
                SaleDate = DateTime.Now,
                Customer = "Existing Customer",
                Branch = "Existing Branch",
                IsCancelled = true, // Mark as cancelled
                Items = new List<UpdateSaleItemCommand>()
            };

            var handler = new UpdateSaleCommandHandler(_mockSaleRepository.Object, _mockMediator.Object, _mockUpdateLogger.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockSaleRepository.Verify(r => r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<SaleCancelledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "DeleteSaleCommandHandler should publish SaleCancelledEvent")]
        public async Task DeleteSaleCommandHandler_ShouldPublishSaleCancelledEvent()
        {
            // Arrange
            var saleId = Guid.NewGuid();
            var existingSale = new Sale(1, DateTime.Now, "Customer", "Branch");
            _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSale);
            _mockSaleRepository.Setup(r => r.DeleteAsync(saleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new DeleteSaleCommand { Id = saleId };
            var handler = new DeleteSaleCommandHandler(_mockSaleRepository.Object, _mockMediator.Object, _mockDeleteLogger.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockSaleRepository.Verify(r => r.DeleteAsync(saleId, It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.IsAny<SaleCancelledEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
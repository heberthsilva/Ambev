using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration
{
    public class SaleIntegrationTests : IDisposable
    {
        private readonly DefaultContext _context;
        private readonly SaleRepository _saleRepository;

        public SaleIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: $"SaleTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new DefaultContext(options);
            _saleRepository = new SaleRepository(_context);

            // Ensure the database is clean for each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact(DisplayName = "Should create a sale and retrieve it from the database")]
        public async Task CreateSale_ShouldPersistAndRetrieve()
        {
            // Arrange
            var sale = new Sale(1, DateTime.Now, "Integration Test Customer", "Integration Branch");
            sale.AddItem(new SaleItem(Guid.NewGuid(), "Product A", 5, 100.00m));

            // Act
            await _saleRepository.CreateAsync(sale, CancellationToken.None);

            // Assert
            var retrievedSale = await _saleRepository.GetByIdAsync(sale.Id, CancellationToken.None);
            Assert.NotNull(retrievedSale);
            Assert.Equal(sale.SaleNumber, retrievedSale.SaleNumber);
            Assert.Single(retrievedSale.Items);
            Assert.Equal(sale.Items.First().Quantity, retrievedSale.Items.First().Quantity);
        }

        [Fact(DisplayName = "Should update an existing sale in the database")]
        public async Task UpdateSale_ShouldUpdateInDatabase()
        {
            // Arrange
            var sale = new Sale(2, DateTime.Now, "Original Customer", "Original Branch");
            sale.AddItem(new SaleItem(Guid.NewGuid(), "Item 1", 1, 10));
            await _saleRepository.CreateAsync(sale, CancellationToken.None);

            var updatedCustomer = "Updated Customer";
            sale.UpdateSaleDetails(sale.SaleNumber, sale.SaleDate, updatedCustomer, sale.Branch);

            // Act
            await _saleRepository.UpdateAsync(sale, CancellationToken.None);

            // Assert
            var retrievedSale = await _saleRepository.GetByIdAsync(sale.Id, CancellationToken.None);
            Assert.NotNull(retrievedSale);
            Assert.Equal(updatedCustomer, retrievedSale.Customer);
        }

        [Fact(DisplayName = "Should delete a sale from the database")]
        public async Task DeleteSale_ShouldRemoveFromDatabase()
        {
            // Arrange
            var sale = new Sale(3, DateTime.Now, "Customer to Delete", "Branch to Delete");
            await _saleRepository.CreateAsync(sale, CancellationToken.None);

            // Act
            var success = await _saleRepository.DeleteAsync(sale.Id, CancellationToken.None);

            // Assert
            Assert.True(success);
            var retrievedSale = await _saleRepository.GetByIdAsync(sale.Id, CancellationToken.None);
            Assert.Null(retrievedSale);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
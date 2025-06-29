using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Ambev.DeveloperEvaluation.Functional
{
    public class SaleFunctionalTests : IClassFixture<CustomWebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program>>
    {
        private readonly HttpClient _client;

        public SaleFunctionalTests(CustomWebApplicationFactory<Ambev.DeveloperEvaluation.WebApi.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(DisplayName = "Should create a new sale successfully")]
        public async Task CreateSale_ShouldReturnSuccess()
        {
            // Arrange
            var request = new
            {
                saleNumber = 1001,
                saleDate = DateTime.Now,
                customer = "Functional Test Customer",
                branch = "Main Branch",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product A", quantity = 5, unitPrice = 100.00m }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/sales", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            Assert.NotNull(result.id);
            Assert.Equal(request.saleNumber, (int)result.saleNumber);
            Assert.Equal(request.customer, (string)result.customer);
        }

        [Fact(DisplayName = "Should get a sale by ID successfully")]
        public async Task GetSaleById_ShouldReturnSale()
        {
            // Arrange - Create a sale first
            var createRequest = new
            {
                saleNumber = 1002,
                saleDate = DateTime.Now,
                customer = "Get Test Customer",
                branch = "Branch 2",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product B", quantity = 3, unitPrice = 50.00m }
                }
            };
            var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sales", createContent);
            createResponse.EnsureSuccessStatusCode();
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            dynamic createResult = JsonConvert.DeserializeObject(createResponseString);
            Guid saleId = createResult.id;

            // Act
            var getResponse = await _client.GetAsync($"/api/sales/{saleId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            var getResponseString = await getResponse.Content.ReadAsStringAsync();
            dynamic getResult = JsonConvert.DeserializeObject(getResponseString);
            Assert.Equal(saleId, (Guid)getResult.id);
            Assert.Equal(createRequest.saleNumber, (int)getResult.saleNumber);
        }

        [Fact(DisplayName = "Should update an existing sale successfully")]
        public async Task UpdateSale_ShouldReturnSuccess()
        {
            // Arrange - Create a sale first
            var createRequest = new
            {
                saleNumber = 1003,
                saleDate = DateTime.Now,
                customer = "Update Test Customer",
                branch = "Branch 3",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product C", quantity = 7, unitPrice = 20.00m }
                }
            };
            var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sales", createContent);
            createResponse.EnsureSuccessStatusCode();
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            dynamic createResult = JsonConvert.DeserializeObject(createResponseString);
            Guid saleId = createResult.id;
            Guid itemId = createResult.items[0].id;

            var updateRequest = new
            {
                id = saleId,
                saleNumber = 1003,
                saleDate = DateTime.Now,
                customer = "Updated Customer Name",
                branch = "Updated Branch",
                isCancelled = false,
                items = new[]
                {
                    new { id = itemId, productId = createRequest.items[0].productId, productName = "Product C", quantity = 8, unitPrice = 20.00m }
                }
            };
            var updateContent = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

            // Act
            var updateResponse = await _client.PutAsync($"/api/sales/{saleId}", updateContent);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            var updateResponseString = await updateResponse.Content.ReadAsStringAsync();
            dynamic updateResult = JsonConvert.DeserializeObject(updateResponseString);
            Assert.Equal(updateRequest.customer, (string)updateResult.customer);
            Assert.Equal(updateRequest.items[0].quantity, (int)updateResult.items[0].quantity);
        }

        [Fact(DisplayName = "Should cancel a sale successfully")]
        public async Task CancelSale_ShouldReturnSuccess()
        {
            // Arrange - Create a sale first
            var createRequest = new
            {
                saleNumber = 1004,
                saleDate = DateTime.Now,
                customer = "Cancel Test Customer",
                branch = "Branch 4",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product D", quantity = 2, unitPrice = 10.00m }
                }
            };
            var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sales", createContent);
            createResponse.EnsureSuccessStatusCode();
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            dynamic createResult = JsonConvert.DeserializeObject(createResponseString);
            Guid saleId = createResult.id;

            var cancelRequest = new
            {
                id = saleId,
                saleNumber = 1004,
                saleDate = DateTime.Now,
                customer = "Cancel Test Customer",
                branch = "Branch 4",
                isCancelled = true, // Mark as cancelled
                items = new object[] { } // No items needed for cancellation
            };
            var cancelContent = new StringContent(JsonConvert.SerializeObject(cancelRequest), Encoding.UTF8, "application/json");

            // Act
            var cancelResponse = await _client.PutAsync($"/api/sales/{saleId}", cancelContent);

            // Assert
            cancelResponse.EnsureSuccessStatusCode();
            var cancelResponseString = await cancelResponse.Content.ReadAsStringAsync();
            dynamic cancelResult = JsonConvert.DeserializeObject(cancelResponseString);
            Assert.True((bool)cancelResult.isCancelled);
        }

        [Fact(DisplayName = "Should delete a sale successfully")]
        public async Task DeleteSale_ShouldReturnSuccess()
        {
            // Arrange - Create a sale first
            var createRequest = new
            {
                saleNumber = 1005,
                saleDate = DateTime.Now,
                customer = "Delete Test Customer",
                branch = "Branch 5",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product E", quantity = 1, unitPrice = 5.00m }
                }
            };
            var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json");
            var createResponse = await _client.PostAsync("/api/sales", createContent);
            createResponse.EnsureSuccessStatusCode();
            var createResponseString = await createResponse.Content.ReadAsStringAsync();
            dynamic createResult = JsonConvert.DeserializeObject(createResponseString);
            Guid saleId = createResult.id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/sales/{saleId}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            var deleteResponseString = await deleteResponse.Content.ReadAsStringAsync();
            dynamic deleteResult = JsonConvert.DeserializeObject(deleteResponseString);
            Assert.True((bool)deleteResult.success);

            // Verify it's actually deleted
            var getResponse = await _client.GetAsync($"/api/sales/{saleId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact(DisplayName = "Should return 400 for invalid quantity (above 20) on create")]
        public async Task CreateSale_ShouldReturnBadRequest_ForQuantityAbove20()
        {
            // Arrange
            var request = new
            {
                saleNumber = 1006,
                saleDate = DateTime.Now,
                customer = "Invalid Quantity Customer",
                branch = "Branch 6",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product F", quantity = 21, unitPrice = 10.00m }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/sales", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Não é possível vender mais de 20 itens idênticos.", responseString);
        }

        [Fact(DisplayName = "Should apply 10% discount for quantity between 4 and 9 on create")]
        public async Task CreateSale_ShouldApply10PercentDiscount()
        {
            // Arrange
            var request = new
            {
                saleNumber = 1007,
                saleDate = DateTime.Now,
                customer = "Discount 10% Customer",
                branch = "Branch 7",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product G", quantity = 5, unitPrice = 100.00m }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/sales", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            Assert.Equal(50.00m, (decimal)result.items[0].discount); // 5 * 100 * 0.10 = 50
            Assert.Equal(450.00m, (decimal)result.items[0].totalItemAmount); // (5 * 100) - 50 = 450
        }

        [Fact(DisplayName = "Should apply 20% discount for quantity between 10 and 20 on create")]
        public async Task CreateSale_ShouldApply20PercentDiscount()
        {
            // Arrange
            var request = new
            {
                saleNumber = 1008,
                saleDate = DateTime.Now,
                customer = "Discount 20% Customer",
                branch = "Branch 8",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product H", quantity = 15, unitPrice = 100.00m }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/sales", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            Assert.Equal(300.00m, (decimal)result.items[0].discount); // 15 * 100 * 0.20 = 300
            Assert.Equal(1200.00m, (decimal)result.items[0].totalItemAmount); // (15 * 100) - 300 = 1200
        }

        [Fact(DisplayName = "Should not apply discount for quantity below 4 on create")]
        public async Task CreateSale_ShouldNotApplyDiscount()
        {
            // Arrange
            var request = new
            {
                saleNumber = 1009,
                saleDate = DateTime.Now,
                customer = "No Discount Customer",
                branch = "Branch 9",
                items = new[]
                {
                    new { productId = Guid.NewGuid(), productName = "Product I", quantity = 3, unitPrice = 100.00m }
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/sales", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseString);
            Assert.Equal(0.00m, (decimal)result.items[0].discount);
            Assert.Equal(300.00m, (decimal)result.items[0].totalItemAmount); // 3 * 100 = 300
        }
    }
}
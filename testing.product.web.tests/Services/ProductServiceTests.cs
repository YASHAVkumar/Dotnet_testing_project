using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using testing_web;

namespace testing.product.web.tests.ProductServiceTests
{
    public sealed class ProductServiceTests
    {
        public readonly Mock<IProductRepo> _mockProductRepo;
        public readonly Mock<ILogger<ProductService>> _mockLogger;
        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepo>();
            _mockLogger = new Mock<ILogger<ProductService>>();
        }

        [Fact]
        public void GetProductsAsync_ReturnProducts()
        {
            //Arrange
            var productsList = new List<Product>()
            {
                new Product
        {
            Id = 1,
            Name = "Laptop",
            Desc = "Portable workstation",
            Price = 500,
            IsActive = true
        },new Product
        {
            Id = 2,
            Name = "charger",
            Desc = "Portable workstation",
            Price = 200,
            IsActive = false
        }
            };
            _mockProductRepo.Setup(x => x.GetProducts()).ReturnsAsync(productsList);

            var service = new ProductService(_mockLogger.Object, _mockProductRepo.Object);

            //act
            var Result = service.GetProductsAsync();
            Assert.NotNull(Result);


        }

        [Fact]
        public async Task GetProductAsync_ProductExists_ReturnsProduct()
        {
            // Arrange

            var product = new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Desc = "Cotton T-Shirt",
                Price = 500,
                IsActive = true
            };

            _mockProductRepo
                .Setup(x => x.GetProductById(1))
                .ReturnsAsync(product);

            var service = new ProductService(_mockLogger.Object, _mockProductRepo.Object);


            // Act

            var result =
                await service.GetProductAsync(1);


            // Assert

            Assert.NotNull(result);

            Assert.Equal(1, result.Id);
            Assert.Equal("T-Shirt", result.Name);
            Assert.Equal(500, result.Price);

            _mockProductRepo.Verify(x => x.GetProductById(1), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetProductAsync_InvalidId_ReturnsNull(int id)
        {
            // Arrange

            var service = new ProductService(_mockLogger.Object, _mockProductRepo.Object);


            // Act

            var result =
                await service.GetProductAsync(id);


            // Assert

            Assert.Equal("",result.Name);

            _mockProductRepo.Verify(
                x => x.GetProductById(
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task GetProductAsync_RepositoryThrows_ReturnsNull()
        {
            // Arrange

            var exception =
                new Exception("Database failed");

            _mockProductRepo
                .Setup(x => x.GetProductById(1))
                .ThrowsAsync(exception);

            var service = new ProductService(_mockLogger.Object, _mockProductRepo.Object);

            // Act

            var result =
                await service.GetProductAsync(1);


            // Assert

            Assert.Null(result);
            _mockLogger.Verify(
       x => x.Log(
           LogLevel.Error,
           It.IsAny<EventId>(),
           It.Is<It.IsAnyType>(
               (state, type) =>
                   state.ToString()!
                       .Contains(
                           "Error fetching product with ID")),
           exception,
           It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
       Times.Once);
        }
    }
}
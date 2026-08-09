using Microsoft.Extensions.Logging;
using Moq;
using testing_web;

namespace testing.web.tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductAsync_WithValidId_ReturnsProduct()
    {
        var product = NewProduct(id: 1);
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        mockRepo.Setup(repo => repo.GetProductById(1)).ReturnsAsync(product);
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.GetProductAsync(1);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        mockRepo.Verify(repo => repo.GetProductById(1), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetProductAsync_WithInvalidId_ReturnsNullAndDoesNotCallRepository(int id)
    {
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.GetProductAsync(id);

        Assert.Null(result);
        mockRepo.Verify(repo => repo.GetProductById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetProductAsync_WhenRepositoryThrows_LogsErrorAndReturnsNull()
    {
        var expectedException = new InvalidOperationException("database failed");
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        mockRepo.Setup(repo => repo.GetProductById(1)).ThrowsAsync(expectedException);
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.GetProductAsync(1);

        Assert.Null(result);
        mockLogger.VerifyLog(
            LogLevel.Error,
            "Error fetching product with ID 1",
            expectedException);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsRepositoryProducts()
    {
        var products = new List<Product> { NewProduct(id: 1), NewProduct(id: 2, name: "Mouse") };
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        mockRepo.Setup(repo => repo.GetProducts()).ReturnsAsync(products);
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.GetProductsAsync();

        Assert.Equal(2, result.Count);
        mockRepo.Verify(repo => repo.GetProducts(), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsCreatedProduct()
    {
        var product = NewProduct();
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        mockRepo.Setup(repo => repo.CreateProduct(product)).ReturnsAsync(product);
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.CreateProductAsync(product);

        Assert.Equal(product.Name, result.Name);
        mockRepo.Verify(repo => repo.CreateProduct(product), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_WithInvalidId_ReturnsFalseAndDoesNotCallRepository()
    {
        var product = NewProduct(id: 0);
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.UpdateProductAsync(product);

        Assert.False(result);
        mockRepo.Verify(repo => repo.UpdateProduct(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProductAsync_ReturnsRepositoryResult()
    {
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        mockRepo.Setup(repo => repo.DeleteProduct(1)).ReturnsAsync(true);
        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        var result = await service.DeleteProductAsync(1);

        Assert.True(result);
        mockRepo.Verify(repo => repo.DeleteProduct(1), Times.Once);
    }

    private static Product NewProduct(int id = 0, string name = "Laptop")
    {
        return new Product
        {
            Id = id,
            Name = name,
            Desc = $"{name} description",
            Date = new DateTime(2026, 1, 1),
            IsActive = true
        };
    }
}

public static class LoggerVerifyExtensions
{
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> loggerMock,
        LogLevel logLevel,
        string expectedMessage,
        Exception? expectedException = null,
        Times? times = null)
    {
        times ??= Times.Once();

        loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString() == expectedMessage),
                It.Is<Exception?>(ex => expectedException == null || ReferenceEquals(ex, expectedException)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times.Value);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using testing_web;
using Xunit;

namespace testing.web.tests;

public class ProductServiceTests
{
    public static IEnumerable<object[]> GetProductTestData()
    {
        // Case 1: Success scenario (Returns product)
        yield return new object[] { 1, new Product { Id = 1, Name = "Laptop" }, null! };

        // Case 2: Exception scenario (Repo throws exception -> Service catches and returns null)
        yield return new object[] { 0, null!, typeof(InvalidOperationException) };
    }

    [Theory]
    [MemberData(nameof(GetProductTestData))]
    public async Task GetProductAsync_ReturnExpectedResult(int id, Product? expectedProduct, Type? exceptionType)
    {
        // Arrange
        var mockRepo = new Mock<IProductRepo>();
        var mockLogger = new Mock<ILogger<ProductService>>();
        Exception? expectedException = null;

        if (exceptionType != null)
        {
            // Configure repository to throw exception
            expectedException = (Exception)Activator.CreateInstance(exceptionType)!;
            mockRepo.Setup(r => r.GetProductById(id)).ThrowsAsync(expectedException);
        }
        else
        {
            // Configure repository to return product
            mockRepo.Setup(r => r.GetProductById(id)).ReturnsAsync(expectedProduct);
        }

        var service = new ProductService(mockLogger.Object, mockRepo.Object);

        // Act
        var result = await service.GetProductAsync(id);

        // Assert
        if (exceptionType != null)
        {
            // Verify return value is null when an exception occurs
            Assert.Null(result);

            // Verify logger recorded the exception using your extension method
            mockLogger.VerifyLog(
                LogLevel.Error,
                $"Error fetching product with ID {id}",
                expectedException
            );
        }
        else
        {
            // Verify product matches expected result
            Assert.NotNull(result);
            Assert.Equal(expectedProduct!.Id, result.Id);
        }
    }

    #region Api testing products endpoint

    #endregion


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
                It.Is<It.IsAnyType>((state, t) => state.ToString() == expectedMessage),
                It.Is<Exception?>(ex => expectedException == null || ReferenceEquals(ex, expectedException)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times.Value);
    }
}
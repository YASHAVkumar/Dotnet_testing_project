using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using testing_web;

namespace testing.web.tests;

public class ProductRepoTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private Mock<ILogger<ProductRepo>> CreateLogger()
    {
        return new Mock<ILogger<ProductRepo>>();
    }


}
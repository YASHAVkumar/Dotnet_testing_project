using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using testing_web;

namespace testing.product.web.tests.Repositories;

public class ProductRepoInMemoryTests
{
    private AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        return new AppDbContext(options);
    }

    private Mock<ILogger<ProductRepo>> CreateLogger()
    {
        return new Mock<ILogger<ProductRepo>>();
    }


    [Fact]
    public async Task GetProductById_ProductExists_ReturnsProduct()
    {
        using var context = CreateContext();

        context.Products.Add(
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Desc = "Cotton T-Shirt",
                Price = 500,
                IsActive = true
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.GetProductById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("T-Shirt", result.Name);
        Assert.Equal(500, result.Price);
    }


    [Fact]
    public async Task GetProductById_ProductDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.GetProductById(999);

        Assert.Null(result);
    }


    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        using var context = CreateContext();

        context.Products.AddRange(
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 500
            },
            new Product
            {
                Id = 2,
                Name = "Jeans",
                Price = 1200
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.GetProducts();

        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            x => x.Name == "T-Shirt");

        Assert.Contains(
            result,
            x => x.Name == "Jeans");
    }


    [Fact]
    public async Task CreateProduct_ValidProduct_SavesProduct()
    {
        using var context = CreateContext();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var product = new Product
        {
            Id = 1,
            Name = "T-Shirt",
            Price = 500
        };

        var result =
            await repository.CreateProduct(product);

        Assert.NotNull(result);

        var savedProduct =
            await context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == 1);

        Assert.NotNull(savedProduct);
        Assert.Equal(
            "T-Shirt",
            savedProduct.Name);
    }


    [Fact]
    public async Task UpdateProduct_ProductExists_UpdatesProduct()
    {
        using var context = CreateContext();

        context.Products.Add(
            new Product
            {
                Id = 1,
                Name = "Old T-Shirt",
                Price = 500
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var product = new Product
        {
            Id = 1,
            Name = "New T-Shirt",
            Price = 700
        };

        var result =
            await repository.UpdateProduct(product);

        Assert.True(result);

        var savedProduct =
            await context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == 1);

        Assert.NotNull(savedProduct);

        Assert.Equal(
            "New T-Shirt",
            savedProduct.Name);

        Assert.Equal(
            700,
            savedProduct.Price);
    }


    [Fact]
    public async Task UpdateProduct_ProductDoesNotExist_ReturnsFalse()
    {
        using var context = CreateContext();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var product = new Product
        {
            Id = 999,
            Name = "Unknown",
            Price = 500
        };

        var result =
            await repository.UpdateProduct(product);

        Assert.False(result);

        Assert.Equal(
            0,
            await context.Products.CountAsync());
    }


    [Fact]
    public async Task DeleteProduct_ProductExists_RemovesProduct()
    {
        using var context = CreateContext();

        context.Products.Add(
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 500
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.DeleteProduct(1);

        Assert.True(result);

        var product =
            await context.Products
                .FirstOrDefaultAsync(
                    x => x.Id == 1);

        Assert.Null(product);
    }


    [Fact]
    public async Task DeleteProduct_ProductDoesNotExist_ReturnsFalse()
    {
        using var context = CreateContext();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.DeleteProduct(999);

        Assert.False(result);

        Assert.Equal(
            0,
            await context.Products.CountAsync());
    }


    [Theory]
    [InlineData(1, true)]
    [InlineData(999, false)]
    public async Task ProductExists_ReturnsCorrectResult(
        int id,
        bool expected)
    {
        using var context = CreateContext();

        context.Products.Add(
            new Product
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 500
            });

        await context.SaveChangesAsync();

        var repository = new ProductRepo(
            context,
            CreateLogger().Object);

        var result =
            await repository.ProductExists(id);

        Assert.Equal(expected, result);
    }
}
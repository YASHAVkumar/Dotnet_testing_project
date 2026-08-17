using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using testing_web;

namespace testing.product.web.tests.Integration;

public abstract class IntegrationTestBase
{
    protected string ConnectionString { get; set; } = Environment.GetEnvironmentVariable(
        "TEST_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "TEST_CONNECTION_STRING environment variable is not configured.");
    //"data source=MYRA\\MSSQLSERVER01;initial catalog=LocalStorage;user id=sa;password=password;TrustServerCertificate=True;Connect Timeout=60;Max Pool Size=50;MultipleActiveResultSets=true;";

    protected async Task ResetDatabaseAsync()
    {
        await using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        const string sql = """
            DELETE FROM Products;

            DBCC CHECKIDENT ('Products', RESEED, 0);
            """;

        await using var command =
            new SqlCommand(sql, connection);

        await command.ExecuteNonQueryAsync();
    }

    protected IConfiguration CreateConfiguration()
    {
        var settings =
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"]
                    = ConnectionString
            };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    protected ProductRespositorySqlClient CreateRepository()
    {
        return new ProductRespositorySqlClient(
            CreateConfiguration());
    }
}


public class ProductRepositorySqlClientTests
    : IntegrationTestBase
{
    // =========================================================
    // CREATE
    // =========================================================

    [Fact]
    public async Task CreateProduct_Should_Insert_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product = new Product
        {
            Name = "T-Shirt",
            Date = DateTime.UtcNow,
            Desc = "Cotton T-Shirt",
            IsActive = true
        };

        // Act
        var result =
            await repository.CreateProduct(product);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("T-Shirt", result.Name);
        Assert.Equal("Cotton T-Shirt", result.Desc);
        Assert.True(result.IsActive);
    }


    // =========================================================
    // GET BY ID
    // =========================================================

    [Fact]
    public async Task GetProductById_Should_Return_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var created =
            await repository.CreateProduct(
                new Product
                {
                    Name = "Jeans",
                    Date = DateTime.UtcNow,
                    Desc = "Blue Jeans",
                    IsActive = true
                });

        // Act
        var result =
            await repository.GetProductById(created.Id);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            created.Id,
            result.Id);

        Assert.Equal(
            "Jeans",
            result.Name);

        Assert.Equal(
            "Blue Jeans",
            result.Desc);

        Assert.True(result.IsActive);
    }


    [Fact]
    public async Task GetProductById_Should_Return_Null_When_Product_Does_Not_Exist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        // Act
        var result =
            await repository.GetProductById(999999);

        // Assert
        Assert.Null(result);
    }


    // =========================================================
    // GET PRODUCTS
    // =========================================================

    [Fact]
    public async Task GetProducts_Should_Return_All_Products()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        await repository.CreateProduct(
            new Product
            {
                Name = "T-Shirt",
                Date = DateTime.UtcNow,
                Desc = "Cotton",
                IsActive = true
            });

        await repository.CreateProduct(
            new Product
            {
                Name = "Jeans",
                Date = DateTime.UtcNow,
                Desc = "Blue",
                IsActive = true
            });

        await repository.CreateProduct(
            new Product
            {
                Name = "Pant",
                Date = DateTime.UtcNow,
                Desc = "Black",
                IsActive = true
            });

        // Act
        var result =
            await repository.GetProducts();

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Contains(
            result,
            x => x.Name == "T-Shirt");

        Assert.Contains(
            result,
            x => x.Name == "Jeans");

        Assert.Contains(
            result,
            x => x.Name == "Pant");
    }


    [Fact]
    public async Task GetProducts_Should_Return_Empty_When_No_Products_Exist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        // Act
        var result =
            await repository.GetProducts();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    // =========================================================
    // EXISTS
    // =========================================================

    [Fact]
    public async Task ProductExists_Should_Return_True_For_Existing_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product =
            await repository.CreateProduct(
                new Product
                {
                    Name = "Shirt",
                    Date = DateTime.UtcNow,
                    IsActive = true
                });

        // Act
        var result =
            await repository.ProductExists(product.Id);

        // Assert
        Assert.True(result);
    }


    [Fact]
    public async Task ProductExists_Should_Return_False_For_NonExisting_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        // Act
        var result =
            await repository.ProductExists(999999);

        // Assert
        Assert.False(result);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    [Fact]
    public async Task UpdateProduct_Should_Update_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product =
            await repository.CreateProduct(
                new Product
                {
                    Name = "Old Shirt",
                    Date = DateTime.UtcNow,
                    Desc = "Old description",
                    IsActive = true
                });

        product.Name = "Updated Shirt";
        product.Desc = "Updated description";

        // Act
        var result =
            await repository.UpdateProduct(product);

        // Assert
        Assert.True(result);

        var updated =
            await repository.GetProductById(product.Id);

        Assert.NotNull(updated);

        Assert.Equal(
            "Updated Shirt",
            updated.Name);

        Assert.Equal(
            "Updated description",
            updated.Desc);
    }


    [Fact]
    public async Task UpdateProduct_Should_Return_False_When_Product_Does_Not_Exist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product = new Product
        {
            Id = 999999,
            Name = "Does Not Exist",
            Date = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result =
            await repository.UpdateProduct(product);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public async Task UpdateProduct_Should_Update_IsActive()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product =
            await repository.CreateProduct(
                new Product
                {
                    Name = "Shirt",
                    Date = DateTime.UtcNow,
                    IsActive = true
                });

        product.IsActive = false;

        // Act
        var result =
            await repository.UpdateProduct(product);

        // Assert
        Assert.True(result);

        var updated =
            await repository.GetProductById(product.Id);

        Assert.NotNull(updated);
        Assert.False(updated.IsActive);
    }


    // =========================================================
    // DELETE
    // =========================================================

    [Fact]
    public async Task DeleteProduct_Should_Soft_Delete_Product()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        var product =
            await repository.CreateProduct(
                new Product
                {
                    Name = "Delete Me",
                    Date = DateTime.UtcNow,
                    IsActive = true
                });

        // Act
        var result =
            await repository.DeleteProduct(product.Id);

        // Assert
        Assert.True(result);

        var deleted =
            await repository.GetProductById(product.Id);

        Assert.NotNull(deleted);

        Assert.False(deleted.IsActive);
    }


    [Fact]
    public async Task DeleteProduct_Should_Return_False_When_Product_Does_Not_Exist()
    {
        // Arrange
        await ResetDatabaseAsync();

        var repository = CreateRepository();

        // Act
        var result =
            await repository.DeleteProduct(999999);

        // Assert
        Assert.False(result);
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using testing_api;
using testing_web;

namespace testing.web.tests;

public sealed class ProductsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"ProductsApiTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Desc = "Portable workstation",
                    Date = new DateTime(2026, 1, 1),
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse",
                    Desc = "Wireless mouse",
                    Date = new DateTime(2026, 1, 2),
                    IsActive = false
                });
            context.SaveChanges();
        });
    }
}

public class ProductsApiIntegrationTests
{
    private const string ProductsEndpoint = "/api/Products";

    [Fact]
    public async Task GetProducts_ReturnsSeededProducts()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(ProductsEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
        Assert.Contains(products, product => product.Id == 1 && product.Name == "Laptop");
        Assert.Contains(products, product => product.Id == 2 && product.Name == "Mouse");
    }

    [Fact]
    public async Task GetProduct_WithExistingId_ReturnsProduct()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"{ProductsEndpoint}/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("Laptop", product.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task GetProduct_WithInvalidOrMissingId_ReturnsNotFound(int id)
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"{ProductsEndpoint}/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithValidPayload_ReturnsCreatedProductAndPersistsIt()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();
        var request = NewProduct("Keyboard", "Mechanical keyboard");

        var response = await client.PostAsJsonAsync(ProductsEndpoint, request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.True(createdProduct.Id > 0);
        Assert.Equal(request.Name, createdProduct.Name);
        Assert.Equal(request.Desc, createdProduct.Desc);

        var persistedProduct = await client.GetFromJsonAsync<Product>($"{ProductsEndpoint}/{createdProduct.Id}");
        Assert.NotNull(persistedProduct);
        Assert.Equal(createdProduct.Id, persistedProduct.Id);
        Assert.Equal(request.Name, persistedProduct.Name);
    }

    [Fact]
    public async Task PostProduct_WithMalformedJson_ReturnsBadRequest()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();
        using var content = new StringContent("{", Encoding.UTF8, "application/json");

        var response = await client.PostAsync(ProductsEndpoint, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutProduct_WithExistingId_UpdatesProduct()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();
        var request = NewProduct("Laptop Pro", "Updated laptop", id: 1);

        var response = await client.PutAsJsonAsync($"{ProductsEndpoint}/1", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var updatedProduct = await client.GetFromJsonAsync<Product>($"{ProductsEndpoint}/1");
        Assert.NotNull(updatedProduct);
        Assert.Equal("Laptop Pro", updatedProduct.Name);
        Assert.Equal("Updated laptop", updatedProduct.Desc);
    }

    [Fact]
    public async Task PutProduct_WithRouteAndBodyIdMismatch_ReturnsBadRequest()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();
        var request = NewProduct("Wrong Id", "Route id and body id do not match", id: 2);

        var response = await client.PutAsJsonAsync($"{ProductsEndpoint}/1", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutProduct_WithMissingId_ReturnsNotFound()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();
        var request = NewProduct("Missing", "Product does not exist", id: 999);

        var response = await client.PutAsJsonAsync($"{ProductsEndpoint}/999", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithExistingId_RemovesProduct()
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();

        var deleteResponse = await client.DeleteAsync($"{ProductsEndpoint}/2");
        var getResponse = await client.GetAsync($"{ProductsEndpoint}/2");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task DeleteProduct_WithInvalidOrMissingId_ReturnsNotFound(int id)
    {
        using var factory = new ProductsApiFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"{ProductsEndpoint}/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static Product NewProduct(string name, string desc, int id = 0)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Desc = desc,
            Date = new DateTime(2026, 1, 3),
            IsActive = true
        };
    }
}

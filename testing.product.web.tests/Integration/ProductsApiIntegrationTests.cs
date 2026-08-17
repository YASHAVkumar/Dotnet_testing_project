// using System.Net;
// using System.Net.Http.Json;
// using testing.product.web.tests.Repositories;
// using testing_web;

// namespace testing.product.web.tests;

// public class ProductApiIntegrationTests
// {
//     private const string ProductsEndpoint =
//         "/api/Products";


//     // =========================================================
//     // GET ALL
//     // =========================================================

//     [Fact]
//     public async Task GetProducts_ReturnsSeededProducts()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.GetAsync(
//                 ProductsEndpoint);


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.OK,
//             response.StatusCode);

//         var products =
//             await response.Content
//                 .ReadFromJsonAsync<List<Product>>();

//         Assert.NotNull(products);

//         Assert.Equal(3, products.Count);

//         Assert.Contains(
//             products,
//             p => p.Id == 1 &&
//                  p.Name == "Laptop");

//         Assert.Contains(
//             products,
//             p => p.Id == 2 &&
//                  p.Name == "Mouse");

//         Assert.Contains(
//             products,
//             p => p.Id == 3 &&
//                  p.Name == "Keyboard");
//     }


//     // =========================================================
//     // GET BY ID - SUCCESS
//     // =========================================================

//     [Fact]
//     public async Task GetProduct_ExistingId_ReturnsProduct()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/1");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.OK,
//             response.StatusCode);

//         var product =
//             await response.Content
//                 .ReadFromJsonAsync<Product>();

//         Assert.NotNull(product);

//         Assert.Equal(1, product.Id);
//         Assert.Equal("Laptop", product.Name);
//         Assert.Equal(50000, product.Price);
//     }


//     // =========================================================
//     // GET BY ID - NOT FOUND
//     // =========================================================

//     [Theory]
//     [InlineData(999)]
//     [InlineData(1000)]
//     public async Task GetProduct_MissingId_ReturnsNotFound(
//         int id)
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/{id}");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NotFound,
//             response.StatusCode);
//     }


//     // =========================================================
//     // GET BY ID - INVALID ID
//     // =========================================================

//     [Theory]
//     [InlineData(0)]
//     [InlineData(-1)]
//     public async Task GetProduct_InvalidId_ReturnsNotFound(
//         int id)
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/{id}");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.OK,
//             response.StatusCode);
//     }


//     // =========================================================
//     // POST - SUCCESS
//     // =========================================================

//     [Fact]
//     public async Task PostProduct_ValidProduct_CreatesProduct()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();

//         var product = new Product
//         {
//             Name = "T-Shirt",
//             Desc = "Cotton T-Shirt",
//             Price = 500,
//             Date = new DateTime(2026, 1, 4),
//             IsActive = true
//         };


//         // Act

//         var response =
//             await client.PostAsJsonAsync(
//                 ProductsEndpoint,
//                 product);


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.Created,
//             response.StatusCode);

//         var createdProduct =
//             await response.Content
//                 .ReadFromJsonAsync<Product>();

//         Assert.NotNull(createdProduct);

//         Assert.True(
//             createdProduct.Id > 0);

//         Assert.Equal(
//             "T-Shirt",
//             createdProduct.Name);

//         Assert.Equal(
//             "Cotton T-Shirt",
//             createdProduct.Desc);

//         Assert.Equal(
//             500,
//             createdProduct.Price);

//         Assert.NotNull(
//             response.Headers.Location);
//     }


//     // =========================================================
//     // POST - VERIFY PERSISTENCE
//     // =========================================================

//     [Fact]
//     public async Task PostProduct_ValidProduct_CanBeRetrievedAfterCreation()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();

//         var product = new Product
//         {
//             Name = "Hoodie",
//             Desc = "Winter Hoodie",
//             Price = 1200,
//             Date = new DateTime(2026, 1, 4),
//             IsActive = true
//         };


//         // Act

//         var postResponse =
//             await client.PostAsJsonAsync(
//                 ProductsEndpoint,
//                 product);

//         var created =
//             await postResponse.Content
//                 .ReadFromJsonAsync<Product>();

//         Assert.NotNull(created);


//         // Second HTTP request

//         var getResponse =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/{created.Id}");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.OK,
//             getResponse.StatusCode);

//         var retrieved =
//             await getResponse.Content
//                 .ReadFromJsonAsync<Product>();

//         Assert.NotNull(retrieved);

//         Assert.Equal(
//             created.Id,
//             retrieved.Id);

//         Assert.Equal(
//             "Hoodie",
//             retrieved.Name);

//         Assert.Equal(
//             1200,
//             retrieved.Price);
//     }


//     // =========================================================
//     // PUT - SUCCESS
//     // =========================================================

//     [Fact]
//     public async Task PutProduct_ExistingProduct_UpdatesProduct()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();

//         var updatedProduct = new Product
//         {
//             Id = 1,
//             Name = "Gaming Laptop",
//             Desc = "Updated workstation",
//             Price = 75000,
//             Date = new DateTime(2026, 1, 5),
//             IsActive = true
//         };


//         // Act

//         var response =
//             await client.PutAsJsonAsync(
//                 $"{ProductsEndpoint}/1",
//                 updatedProduct);


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NoContent,
//             response.StatusCode);


//         // Verify through another HTTP request

//         var getResponse =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/1");

//         Assert.Equal(
//             HttpStatusCode.OK,
//             getResponse.StatusCode);

//         var product =
//             await getResponse.Content
//                 .ReadFromJsonAsync<Product>();

//         Assert.NotNull(product);

//         Assert.Equal(
//             "Gaming Laptop",
//             product.Name);

//         Assert.Equal(
//             "Updated workstation",
//             product.Desc);

//         Assert.Equal(
//             75000,
//             product.Price);
//     }


//     // =========================================================
//     // PUT - ID MISMATCH
//     // =========================================================

//     [Fact]
//     public async Task PutProduct_RouteIdDoesNotMatchBodyId_ReturnsBadRequest()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();

//         var product = new Product
//         {
//             Id = 2,
//             Name = "Wrong ID",
//             Desc = "Invalid request",
//             Price = 500
//         };


//         // Act

//         var response =
//             await client.PutAsJsonAsync(
//                 $"{ProductsEndpoint}/1",
//                 product);


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.BadRequest,
//             response.StatusCode);
//     }


//     // =========================================================
//     // PUT - PRODUCT DOES NOT EXIST
//     // =========================================================

//     [Fact]
//     public async Task PutProduct_MissingProduct_ReturnsNotFound()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();

//         var product = new Product
//         {
//             Id = 999,
//             Name = "Unknown",
//             Desc = "Does not exist",
//             Price = 500
//         };


//         // Act

//         var response =
//             await client.PutAsJsonAsync(
//                 $"{ProductsEndpoint}/999",
//                 product);


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NotFound,
//             response.StatusCode);
//     }


//     // =========================================================
//     // DELETE - SUCCESS
//     // =========================================================

//     [Fact]
//     public async Task DeleteProduct_ExistingProduct_DeletesProduct()
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var deleteResponse =
//             await client.DeleteAsync(
//                 $"{ProductsEndpoint}/2");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NoContent,
//             deleteResponse.StatusCode);


//         // Verify deletion through GET

//         var getResponse =
//             await client.GetAsync(
//                 $"{ProductsEndpoint}/2");

//         Assert.Equal(
//             HttpStatusCode.NotFound,
//             getResponse.StatusCode);
//     }


//     // =========================================================
//     // DELETE - MISSING PRODUCT
//     // =========================================================

//     [Theory]
//     [InlineData(999)]
//     [InlineData(1000)]
//     public async Task DeleteProduct_MissingProduct_ReturnsNotFound(
//         int id)
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.DeleteAsync(
//                 $"{ProductsEndpoint}/{id}");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NotFound,
//             response.StatusCode);
//     }


//     // =========================================================
//     // DELETE - INVALID ID
//     // =========================================================

//     [Theory]
//     [InlineData(0)]
//     [InlineData(-1)]
//     public async Task DeleteProduct_InvalidId_ReturnsNotFound(
//         int id)
//     {
//         // Arrange

//         using var factory =
//             new ProductRepositoryFactory();

//         using var client =
//             factory.CreateClient();


//         // Act

//         var response =
//             await client.DeleteAsync(
//                 $"{ProductsEndpoint}/{id}");


//         // Assert

//         Assert.Equal(
//             HttpStatusCode.NotFound,
//             response.StatusCode);
//     }
// }
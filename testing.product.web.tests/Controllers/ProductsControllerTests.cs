// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;
// using testing_api.Controllers;
// using testing_web;

// namespace testing.product.web.tests.Controllers;

// public class ProductsControllerTests
// {
//     private readonly Mock<IProductRepo> _repoMock;
//     private readonly Mock<ILogger<ProductService>> _loggerMock;

//     public ProductsControllerTests()
//     {
//         _repoMock = new Mock<IProductRepo>();

//         _loggerMock =
//             new Mock<ILogger<ProductService>>();
//     }

//     private ProductsController CreateController()
//     {
//         var service = new ProductService(
//             _loggerMock.Object,
//             _repoMock.Object);

//         return new ProductsController(service);
//     }


//     [Fact]
//     public async Task GetProduct_ProductExists_ReturnsOk()
//     {
//         // Arrange

//         var product = new Product
//         {
//             Id = 1,
//             Name = "Laptop",
//             Desc = "Portable workstation",
//             Price = 500,
//             IsActive = true
//         };

//         _repoMock
//             .Setup(x => x.GetProductById(1))
//             .ReturnsAsync(product);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.GetProduct(1);


//         // Assert

//         var okResult =
//             Assert.IsType<OkObjectResult>(
//                 result.Result);

//         var returnedProduct =
//             Assert.IsType<Product>(
//                 okResult.Value);

//         Assert.Equal(1, returnedProduct.Id);
//         Assert.Equal("Laptop", returnedProduct.Name);
//     }


//     [Fact]
//     public async Task GetProduct_ProductDoesNotExist_ReturnsNotFound()
//     {
//         // Arrange

//         _repoMock
//             .Setup(x => x.GetProductById(999))
//             .ReturnsAsync((Product?)null);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.GetProduct(999);


//         // Assert

//         Assert.IsType<NotFoundResult>(
//             result.Result);
//     }


//     [Theory]
//     [InlineData(0)]
//     [InlineData(-1)]
//     public async Task GetProduct_InvalidId_ReturnsNotFound(int id)
//     {
//         // Arrange

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.GetProduct(id);


//         // Assert

//         Assert.IsType<OkObjectResult>(
//             result.Result);

//         _repoMock.Verify(
//             x => x.GetProductById(
//                 It.IsAny<int>()),
//             Times.Never);
//     }


//     [Fact]
//     public async Task PostProduct_ValidProduct_ReturnsCreated()
//     {
//         // Arrange

//         var product = new Product
//         {
//             Id = 1,
//             Name = "Laptop",
//             Desc = "Portable workstation",
//             Price = 500,
//             IsActive = true
//         };

//         _repoMock
//             .Setup(x => x.CreateProduct(product))
//             .ReturnsAsync(product);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.PostProduct(product);


//         // Assert

//         var createdResult =
//             Assert.IsType<CreatedAtActionResult>(
//                 result.Result);

//         Assert.Equal(
//             nameof(ProductsController.GetProduct),
//             createdResult.ActionName);

//         Assert.Equal(
//             product,
//             createdResult.Value);

//         Assert.Equal(
//             1,
//             createdResult.RouteValues!["id"]);
//     }


//     [Fact]
//     public async Task PutProduct_IdMismatch_ReturnsBadRequest()
//     {
//         // Arrange

//         var product = new Product
//         {
//             Id = 2,
//             Name = "Laptop"
//         };

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.PutProduct(1, product);


//         // Assert

//         Assert.IsType<BadRequestResult>(result);

//         _repoMock.Verify(
//             x => x.UpdateProduct(
//                 It.IsAny<Product>()),
//             Times.Never);
//     }


//     [Fact]
//     public async Task PutProduct_ValidProduct_ReturnsNoContent()
//     {
//         // Arrange

//         var product = new Product
//         {
//             Id = 1,
//             Name = "Laptop Pro"
//         };

//         _repoMock
//             .Setup(x => x.UpdateProduct(product))
//             .ReturnsAsync(true);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.PutProduct(1, product);


//         // Assert

//         Assert.IsType<NoContentResult>(result);

//         _repoMock.Verify(
//             x => x.UpdateProduct(product),
//             Times.Once);
//     }


//     [Fact]
//     public async Task PutProduct_MissingProduct_ReturnsNotFound()
//     {
//         // Arrange

//         var product = new Product
//         {
//             Id = 999,
//             Name = "Unknown"
//         };

//         _repoMock
//             .Setup(x => x.UpdateProduct(product))
//             .ReturnsAsync(false);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.PutProduct(999, product);


//         // Assert

//         Assert.IsType<NotFoundResult>(result);
//     }


//     [Fact]
//     public async Task DeleteProduct_ExistingProduct_ReturnsNoContent()
//     {
//         // Arrange

//         _repoMock
//             .Setup(x => x.DeleteProduct(1))
//             .ReturnsAsync(true);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.DeleteProduct(1);


//         // Assert

//         Assert.IsType<NoContentResult>(result);

//         _repoMock.Verify(
//             x => x.DeleteProduct(1),
//             Times.Once);
//     }


//     [Fact]
//     public async Task DeleteProduct_MissingProduct_ReturnsNotFound()
//     {
//         // Arrange

//         _repoMock
//             .Setup(x => x.DeleteProduct(999))
//             .ReturnsAsync(false);

//         var controller = CreateController();


//         // Act

//         var result =
//             await controller.DeleteProduct(999);


//         // Assert

//         Assert.IsType<NotFoundResult>(result);
//     }
// }
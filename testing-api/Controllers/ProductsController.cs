using Microsoft.AspNetCore.Mvc;
using testing_web;
using static System.Net.Mime.MediaTypeNames;

namespace testing_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(
        ProductService productService,
        IImageStorageService imageStorage) : ControllerBase
    {
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await productService.GetProductsAsync();

            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await productService.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(
            int id,
            Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var updated =
                await productService.UpdateProductAsync(product);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(
            [FromForm] CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Date = request.Date,
                Desc = request.Desc,
                Price = request.Price,
                IsActive = request.IsActive
            };

            // Save images to wwwroot/images
            if (request.Images.Count > 0)
            {
                foreach (var image in request.Images)
                {
                    if (image.Length == 0)
                        continue;

                    await using var stream =
                        image.OpenReadStream();

                    var imagePath =
                        await imageStorage.UploadAsync(
                            stream,
                            image.FileName,
                            image.ContentType,
                            "images");

                    product.ProductImages.Add(
                        new ProductImages
                        {
                            ImageUrl = imagePath
                        });
                }
            }
            // Product + ProductImages
            // are saved together
            var createdProduct =
                await productService.CreateProductAsync(product);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = createdProduct.Id },
                createdProduct);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted =
                await productService.DeleteProductAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{productId}/images/view")]
        public async Task<IActionResult> ViewImages(int productId)
        {
            var product =
                await productService.GetProductAsync(productId);

            if (product == null)
                return NotFound();

            var html = string.Join(
                "",
                await Task.WhenAll(
                    product.ProductImages.Select(async image =>
                    {
                        var url =
                            await imageStorage.GetUrlAsync(
                                image.ImageUrl);

                        return
                            $"<img src='{url}' " +
                            "style='max-width:300px; margin:10px;' />";
                    })
                )
            );

            return Content(html, "text/html");
        }

    }
}
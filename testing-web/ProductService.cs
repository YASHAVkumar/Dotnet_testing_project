using Microsoft.Extensions.Logging;

namespace testing_web;

public class ProductService(ILogger<ProductService> logger, IProductRepo productRepo, IImageStorageService imageStorage)
{
    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await productRepo.GetProducts();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        if (id <= 0)
            return null;

        try
        {
            return await productRepo.GetProductById(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product with ID {Id}", id);
            return null;
        }
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return await productRepo.CreateProduct(product);
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.Id <= 0)
            return false;

        return await productRepo.UpdateProduct(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await productRepo.GetProductById(id);

        if (product == null)
        {
            return false;
        }

        foreach (var image in product.ProductImages)
        {
            await imageStorage.DeleteAsync(image.ImageUrl);
        }

        await productRepo.DeleteProduct(id);

        return true;
    }
}

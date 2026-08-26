using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace testing_web
{
    public class ProductRepo(AppDbContext context, ILogger<ProductRepo> logger) : IProductRepo
    {
        public async Task<IReadOnlyList<Product>> GetProducts()
        {
            try
            {
                return await context.Products
                    .Include(x => x.ProductImages)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching products");
                throw;
            }
        }

        public async Task<Product?> GetProductById(int id)
        {
            try
            {
                return await context.Products
                    .Include(x => x.ProductImages)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error fetching product with ID {Id}",
                    id);

                throw;
            }
        }

        public async Task<Product> CreateProduct(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var existingProduct = await context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (existingProduct == null)
            {
                return false;
            }

            existingProduct.Name = product.Name;
            existingProduct.Desc = product.Desc;
            existingProduct.Price = product.Price;

            // Remove existing image records from DB
            context.ProductImages.RemoveRange(existingProduct.ProductImages);

            // Add new image records to DB
            foreach (var image in product.ProductImages)
            {
                image.ProductId = product.Id;
                context.ProductImages.Add(image);
            }

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return false;
            }

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ProductExists(int id)
        {
            return await context.Products.AnyAsync(product => product.Id == id);
        }
    }
}

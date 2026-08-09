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
            return await context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            try
            {
                return await context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(product => product.Id == id);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching product with ID {Id}", id);
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
            if (!await ProductExists(product.Id))
            {
                return false;
            }

            context.Entry(product).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);
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

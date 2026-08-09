using System;
using System.Collections.Generic;
using System.Text;

namespace testing_web
{
    public interface IProductRepo
    {
        public Task<IReadOnlyList<Product>> GetProducts();
        public Task<Product?> GetProductById(int id);
        public Task<Product> CreateProduct(Product product);
        public Task<bool> UpdateProduct(Product product);
        public Task<bool> DeleteProduct(int id);
        public Task<bool> ProductExists(int id);
    }
}

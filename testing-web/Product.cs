using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace testing_web
{
    public class Product
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Desc { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public bool IsActive { get; set; } = false;
        public ICollection<ProductImages> ProductImages { get; set; } = [];
    }

    public class ProductImages
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Desc { get; set; } = string.Empty;

        public int Price { get; set; }

        public bool IsActive { get; set; }

        public List<IFormFile> Images { get; set; } = [];
    }

    public class UpdateProductRequest
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Desc { get; set; } = string.Empty;

        public int Price { get; set; }

        public bool IsActive { get; set; }

        public List<IFormFile> Images { get; set; } = [];
    }
}

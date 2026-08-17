
using Microsoft.EntityFrameworkCore;
using testing_web;

namespace testing_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            Console.WriteLine($"Connection string: '{connectionString}'");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ProductService>();
            // builder.Services.AddScoped<IProductRepo,ProductRepo>();
            var provider = builder.Configuration["DataAccess:Provider"];

            if (string.Equals(
                    provider,
                    "Ef",
                    StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddScoped<IProductRepo, ProductRepo>();
            }
            else if (string.Equals(
                         provider,
                         "Sql",
                         StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddScoped<IProductRepo, ProductRespositorySqlClient>();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unknown DataAccess provider: {provider}");
            }
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.MapGet("/",()=>"Hello World");
            app.Run();
        }
    }
}

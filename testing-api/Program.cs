
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using testing_web;

namespace testing_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactApp", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            builder.Services.AddSignalR();
            //register aws services 
            builder.Services.AddDefaultAWSOptions(
              builder.Configuration.GetAWSOptions());

            builder.Services.AddAWSService<IAmazonS3>();
            var storageProvider =
    builder.Configuration["Storage:Provider"];

            if (storageProvider == "S3")
            {
                builder.Services.AddScoped<
                    IImageStorageService,
                    S3ImageStorageService>();
            }
            else
            {
                builder.Services.AddScoped<
                    IImageStorageService,
                    LocalImageStorageService>();
            }

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
            app.UseCors("ReactApp");
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllers();
            app.MapGet("/", () => "Hello World");
            app.MapHub<ProductHub>("/hubs/products");
            app.Run();
        }
    }
}

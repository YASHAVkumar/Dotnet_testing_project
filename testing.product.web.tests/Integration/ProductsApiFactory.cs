// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using testing_api;
// using testing_web;

// namespace testing.product.web.tests.Repositories
// {
//     public sealed class ProductRepositoryFactory : WebApplicationFactory<Program>
//     {
//         private readonly string _databaseName = $"ProductsApiTests-{Guid.NewGuid()}";
//         private readonly ServiceProvider _inMemoryServiceProvider = new ServiceCollection()
//                 .AddEntityFrameworkInMemoryDatabase()
//         .BuildServiceProvider();

//         protected override void ConfigureWebHost(IWebHostBuilder builder)
//         {
//             builder.UseEnvironment("Testing");

//             builder.ConfigureServices(services =>
//             {
//                 services.RemoveAll<AppDbContext>();
//                 services.RemoveAll<DbContextOptions>();
//                 services.RemoveAll<DbContextOptions<AppDbContext>>();

//                 services.AddDbContext<AppDbContext>(options =>
//                     options.UseInMemoryDatabase(_databaseName)
//                         .UseInternalServiceProvider(_inMemoryServiceProvider));

//                 using var provider = services.BuildServiceProvider();
//                 using var scope = provider.CreateScope();
//                 var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//                 context.Database.EnsureDeleted();
//                 context.Database.EnsureCreated();
//                 context.Products.AddRange(
//                     new Product
//                     {
//                         Id = 1,
//                         Name = "Laptop",
//                         Desc = "Portable workstation",
//                         Date = new DateTime(2026, 1, 1),
//                         IsActive = true
//                     },
//                     new Product
//                     {
//                         Id = 2,
//                         Name = "Mouse",
//                         Desc = "Wireless mouse",
//                         Date = new DateTime(2026, 1, 2),
//                         IsActive = false
//                     },
//                       new Product
//                       {
//                           Id = 3,
//                           Name = "Data",
//                           Desc = "wahouse data",
//                           Date = new DateTime(2026, 1, 2),
//                           IsActive = true
//                       });
//                 context.SaveChanges();
//             });
//         }

//         protected override void Dispose(bool disposing)
//         {
//             base.Dispose(disposing);

//             if (disposing)
//             {
//                 _inMemoryServiceProvider.Dispose();
//             }
//         }
//     }
// }
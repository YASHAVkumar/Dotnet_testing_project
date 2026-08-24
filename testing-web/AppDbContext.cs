using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace testing_web
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImages> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {

                entity.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Desc)
                    .IsRequired()
                    .HasMaxLength(500);

                modelBuilder.Entity<ProductImages>()
                        .HasOne(x => x.Product)
                        .WithMany(x => x.ProductImages)
                        .HasForeignKey(x => x.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);


            });
        }
    }
}

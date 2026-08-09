using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace testing_web
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

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
            });
        }
    }
}

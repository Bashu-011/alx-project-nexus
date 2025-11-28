using E_Commerce.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        //Cart and cart items
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        //order dbsets
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity setup
            modelBuilder.Entity<User>(entity =>
            {  
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            // Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            });

            // Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.Price);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            //Cart entity
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.IsActive });

                //user can have many carts
                entity.HasOne(e => e.User)
                      .WithMany()  
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });


            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                //indexing
                entity.HasIndex(e => e.CartId);

                //must have quantity
                entity.Property(e => e.Quantity)
                      .IsRequired();

                
                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                //cart has many items
                entity.HasOne(e => e.Cart)
                      .WithMany(c => c.Items)
                      .HasForeignKey(e => e.CartId)
                      .OnDelete(DeleteBehavior.Cascade);  //If cart deleted, delete its items

                //one product un many cart items
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                //indexing 
                entity.HasIndex(e => e.UserId);

                // Indexing for order numbers 
                entity.HasIndex(e => e.OrderNumber).IsUnique();

                //indexing for mpesa tracking
                entity.HasIndex(e => e.MpesaCheckoutRequestId);

                entity.Property(e => e.OrderNumber)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.TotalAmount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.PhoneNumber)
                      .HasMaxLength(15);

                entity.Property(e => e.MpesaReceiptNumber)
                      .HasMaxLength(50);

                entity.Property(e => e.MpesaCheckoutRequestId)
                      .HasMaxLength(100);

                entity.Property(e => e.MpesaMerchantRequestId)
                      .HasMaxLength(100);

                //user has many orders
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                //indexing to get orders
                entity.HasIndex(e => e.OrderId);

                entity.Property(e => e.ProductName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.ProductImageUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.TotalPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                //an order has many items
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); //delete items if order deleted

                //order itemes ref products
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);  //don't delete product if in orders
            });


            // Seed Data
            //SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var categoryId1 = Guid.NewGuid();
            var categoryId2 = Guid.NewGuid();

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = categoryId1,
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    Slug = "electronics",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = categoryId2,
                    Name = "Books",
                    Description = "Books and magazines",
                    Slug = "books",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

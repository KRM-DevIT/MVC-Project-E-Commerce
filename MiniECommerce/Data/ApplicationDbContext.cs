using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniECommerce.Models;
using MiniECommerce.Models.IdentityModels;

namespace MiniECommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        //DbSets (Entities)

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Address> Addresses { get; set; }


        /// <summary>
        /// Constructor receives DbContextOptions which contains configuration like 
        /// which database provider to use (InMemory, SQL Server, etc.)
        /// This is injected by the DI container configured in Program.cs
        /// </summary>

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        /// <summary>
        /// OnModelCreating is where you configure the database schema.
        /// We call base.OnModelCreating(builder) to ensure Identity's default 
        /// configurations are applied (indexes, relationships, constraints).
        /// 
        /// You can add custom configurations here if you extend IdentityUser
        /// or need to modify default behaviors.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>(category => {
                category.HasOne(c => c.ParentCategory)
                        .WithMany(c => c.ChildrenCategories)
                        .HasForeignKey(c => c.ParentCategoryId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });
            builder.Entity<Product>(product =>
            {
                product.HasOne(p => p.Category)
                       .WithMany(p => p.Products)
                       .HasForeignKey(p => p.ProductId)
                       .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Order>(order =>
            {
                order.HasOne(o => o.Address)
                     .WithMany(o => o.Orders)
                     .HasForeignKey(o => o.ShippingAddressId)
                     .OnDelete(DeleteBehavior.Restrict);

                order.HasOne(o=>o.ApplicationUser)
                     .WithMany(o=>o.Orders)
                     .HasForeignKey(o=>o.ApplicationUserId)
                     .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<OrderItem>(item =>
            {
               item.HasOne(i=>i.Order)
                   .WithMany(i=>i.OrderItems)
                   .HasForeignKey(i => i.OrderItemId)
                   .OnDelete(DeleteBehavior.Restrict);

                item.HasOne(i => i.Product)
                    .WithMany(i => i.OrderItems)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Address>(address => 
            {
                address.HasOne(addr => addr.User)
                       .WithMany(User => User.Addresses)
                       .HasForeignKey(addr => addr.UserId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(builder);    
        }
    }
}

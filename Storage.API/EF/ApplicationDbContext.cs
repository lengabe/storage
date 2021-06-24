using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Storage.API.Core;

namespace Storage.API.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreProducts> StoreProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StoreProducts>()
                .HasKey(x => new { x.ProductId, x.StoreId });

            builder.Entity<Store>()
                .HasMany(x => x.StoreProducts)
                .WithOne(x => x.Store)
                .HasForeignKey(x => x.StoreId);

            builder.Entity<StoreProducts>()
                .HasOne(x => x.Product)
                .WithMany(x => x.StoreProducts)
                .HasForeignKey(x => x.ProductId);

            base.OnModelCreating(builder);
        }
    }
}

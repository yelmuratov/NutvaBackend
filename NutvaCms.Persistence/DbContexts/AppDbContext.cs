using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Persistence.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<SiteStatistic> SiteStatistics => Set<SiteStatistic>();
        public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
        public DbSet<PurchaseRequestProduct> PurchaseRequestProducts => Set<PurchaseRequestProduct>();
        public DbSet<TrackingPixel> TrackingPixels => Set<TrackingPixel>();
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<BlogPostMedia> BlogPostMedia => Set<BlogPostMedia>();
        public DbSet<ChatAdmin> ChatAdmins => Set<ChatAdmin>();
        public DbSet<ProductBoxPrice> ProductBoxPrices => Set<ProductBoxPrice>();
        public DbSet<ContactForm> ContactForms => Set<ContactForm>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product image list
            modelBuilder.Entity<Product>()
                .Property(p => p.ImageUrls)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // Banner image list
            modelBuilder.Entity<Banner>()
                .Property(b => b.ImageUrls)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // Translations
            modelBuilder.Entity<Product>().OwnsOne(p => p.En);
            modelBuilder.Entity<Product>().OwnsOne(p => p.Uz);
            modelBuilder.Entity<Product>().OwnsOne(p => p.Ru);
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.En);
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Uz);
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Ru);
            modelBuilder.Entity<Banner>().OwnsOne(b => b.En);
            modelBuilder.Entity<Banner>().OwnsOne(b => b.Uz);
            modelBuilder.Entity<Banner>().OwnsOne(b => b.Ru);

            // Purchase relationship
            modelBuilder.Entity<PurchaseRequest>()
                .HasMany(pr => pr.Products)
                .WithOne(p => p.PurchaseRequest)
                .HasForeignKey(p => p.PurchaseRequestId);

            // ✅ New: Decimal type config
            modelBuilder.Entity<PurchaseRequest>()
                .Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseRequestProduct>()
                .Property(p => p.DiscountedPrice)
                .HasColumnType("decimal(18,2)");

            // Cascade delete: product -> box prices
            modelBuilder.Entity<ProductBoxPrice>()
                .HasOne(pbp => pbp.Product)
                .WithMany(p => p.BoxPrices)
                .HasForeignKey(pbp => pbp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

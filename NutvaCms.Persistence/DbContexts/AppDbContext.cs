using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Persistence.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ✅ All DbSets
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<SiteStatistic> SiteStatistics => Set<SiteStatistic>();
        public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
        public DbSet<PurchaseRequestProduct> PurchaseRequestProducts { get; set; }
        public DbSet<TrackingPixel> TrackingPixels => Set<TrackingPixel>();
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<BlogPostMedia> BlogPostMedia => Set<BlogPostMedia>();
        public DbSet<ChatAdmin> ChatAdmins { get; set; } = null!;

        public DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Product image list serialization
            modelBuilder.Entity<Product>()
                .Property(p => p.ImageUrls)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // ✅ Banner image list serialization
            modelBuilder.Entity<Banner>()
                .Property(b => b.ImageUrls)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            modelBuilder.Entity<ChatSession>()
            .Property(cs => cs.Id)
            .HasDefaultValueSql("gen_random_uuid()");

            // ✅ Product Translations
            modelBuilder.Entity<Product>().OwnsOne(p => p.En);
            modelBuilder.Entity<Product>().OwnsOne(p => p.Uz);
            modelBuilder.Entity<Product>().OwnsOne(p => p.Ru);

            // ✅ BlogPost Translations
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.En);
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Uz);
            modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Ru);

            // ✅ Banner Translations
            modelBuilder.Entity<Banner>().OwnsOne(b => b.En);
            modelBuilder.Entity<Banner>().OwnsOne(b => b.Uz);
            modelBuilder.Entity<Banner>().OwnsOne(b => b.Ru);

            modelBuilder.Entity<PurchaseRequest>()
            .HasMany(pr => pr.Products)
            .WithOne(p => p.PurchaseRequest)
            .HasForeignKey(p => p.PurchaseRequestId);
        }

    }
}

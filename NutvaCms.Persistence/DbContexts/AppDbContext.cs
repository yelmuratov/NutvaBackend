using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;
namespace NutvaCms.Persistence.DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Existing DbSets
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<SiteStatistic> SiteStatistics => Set<SiteStatistic>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<TrackingPixel> TrackingPixels => Set<TrackingPixel>();

    // ✅ New DbSets for Blog module
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogPostMedia> BlogPostMedia => Set<BlogPostMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Existing Product image conversion
        modelBuilder.Entity<Product>()
            .Property(p => p.ImageUrls)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        modelBuilder.Entity<Banner>()
            .Property(b => b.ImageUrls)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        // ✅ OWNED TRANSLATIONS (IMPORTANT PART!)
        modelBuilder.Entity<BlogPost>().OwnsOne(b => b.En);
        modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Uz);
        modelBuilder.Entity<BlogPost>().OwnsOne(b => b.Ru);
    }
}

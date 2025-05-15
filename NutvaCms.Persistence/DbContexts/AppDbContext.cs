using Microsoft.EntityFrameworkCore;
using NutvaCms.Domain.Entities;

namespace NutvaCms.Persistence.DbContexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<SiteStatistic> SiteStatistics => Set<SiteStatistic>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<TrackingPixel> TrackingPixels => Set<TrackingPixel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>()
            .Property(b => b.ImageUrls)
            .HasConversion(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList()
            );

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
    }
}


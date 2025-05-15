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
}

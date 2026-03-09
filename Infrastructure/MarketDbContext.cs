using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class MarketDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; }

    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
    {
        
    }
}
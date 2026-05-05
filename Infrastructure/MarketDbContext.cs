using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class MarketDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
    {
        
    }
}
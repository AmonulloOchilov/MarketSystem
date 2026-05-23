using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Data;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Employee> Employees { get; set; }
    DbSet<Admin> Admins { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DbSet<Person> Persons { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
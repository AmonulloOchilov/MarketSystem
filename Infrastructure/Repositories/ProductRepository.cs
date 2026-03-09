using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MarketDbContext _db;

    public ProductRepository(MarketDbContext db)
    {
        _db = db;
    }
    public async Task<List<Product>> GettAllAsync()
    {
        return await _db.Products.Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();
    }
}
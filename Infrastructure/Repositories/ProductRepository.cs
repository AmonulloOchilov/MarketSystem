using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(MarketDbContext db, ILogger<ProductRepository> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<List<Product>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _db.Products.Include(p => p.Category).OrderBy(p => p.Id).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var result = await _db.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (result == null)
        {
            _logger.LogWarning("Product not found in database. Product ID: {ProductId}", id);
            return null;
        }
        return result;
    }

    public async Task<Product> AddAsync(Product product)
    {
        _logger.LogInformation("Adding product to Database. Product ID: {ProductId}, Name {ProductName}, Category ID {CategoryId}",
            product.Id, product.Name, product.CategoryId);
        
        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Product successfully created with ID: {ProductId}", product.Id);
        return product;
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        var existingProduct = await _db.Products.FindAsync(product.Id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Cannot update. Product not found. ProductId: {ProductId}", product.Id);
            return null;
        }
        
        _logger.LogInformation("Updating product. ProductId: {ProductId}", product.Id);
        
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Product updated successfully");
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Cannot delete. Product not found. ProductId: {ProductId}", id);
            return;
        }
        _logger.LogInformation("Deleting product from database. Product ID: {ProductId}", id);
        
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Product deleted from database. Product ID: {ProductId}", id);
    }
}
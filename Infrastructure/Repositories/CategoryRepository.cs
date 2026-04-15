using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(MarketDbContext db, ILogger<CategoryRepository> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<List<Category>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _db.Categories.OrderBy(c => c.Id)
            .Take((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var result = await _db.Categories.FindAsync(id);
        if (result == null)
        {
            _logger.LogWarning("Category not found in database. Category ID: {CategoryId}", id);
            return null;
        }
        return result;
    }

    public async Task<Category> AddAsync(Category category)
    {
        _logger.LogInformation("Adding category to Database. Category ID: {CategoryId}", category.Id);
        
        await _db.Categories.AddAsync(category);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category successfully created with ID: {CategoryId}", category.Id);
        return category;
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        _logger.LogInformation("Updating category. Category ID: {CategoryId}", category.Id);
        
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category updated successfully. Category ID: {CategoryId}", category.Id);
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Cannot delete. Category not found. Category ID: {CategoryId}", id);
            return;
        }
        _logger.LogInformation("Deleting category from database. Category ID: {CategoryId}", id);
        
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category deleted from database. Category ID: {CustomerId}", id);
    }
}
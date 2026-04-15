using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(int pageNumber, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task DeleteAsync(int id);
}
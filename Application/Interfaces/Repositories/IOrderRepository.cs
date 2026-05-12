using Application.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize);
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task UpdateAsync(Order order);
    Task<bool> AnyOrderContainsProductAsync(int productId);
}
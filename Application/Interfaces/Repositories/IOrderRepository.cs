using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync();
    Task<Order> CreateAsync(Order order);

    Task<Order?> GetByIdAsync(int id);
}
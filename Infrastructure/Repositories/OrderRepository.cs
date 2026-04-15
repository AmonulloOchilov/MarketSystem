using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class OrderRepository:IOrderRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(MarketDbContext db, ILogger<OrderRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Order>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _db.Orders.Include(order => order.OrderItems)
            .OrderBy(o => o.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _logger.LogInformation("Adding order to Database. Order ID: {OrderId}, Name {CustomerId}, Category ID {OrderItems}",
            order.Id, order.CustomerId, order.OrderItems);
        
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Order successfully created with ID: {OrderId}", order.Id);
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var result = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (result == null)
        {
            _logger.LogWarning("Order not found in database. Order ID: {ProductId}", id);
            return null;
        }
        return result;
    }
}
using Application.Common;
using Application.Exceptions;
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

    public async Task<PagedResult<Order>> GetAllAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _db.Orders.CountAsync();
        var items = await _db.Orders.Include(order => order.OrderItems)
            .OrderBy(o => o.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<Order>()
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _logger.LogInformation("Adding order to Database. Order ID: {OrderId}, Name {CustomerId}, Category ID {OrderItems}",
            order.Id, order.CustomerId, order.OrderItems);
        
        await _db.Orders.AddAsync(order);
        
        _logger.LogInformation("Order successfully created with ID: {OrderId}", order.Id);
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var result = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
        
        return result;
    }

    public Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task<bool> AnyOrderContainsProductAsync(int productId)
    {
        return await _db.Orders.AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
    }
}
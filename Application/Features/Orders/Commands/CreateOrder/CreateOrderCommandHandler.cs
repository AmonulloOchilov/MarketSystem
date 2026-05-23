using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IAppDbContext dbContext,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.Items == null || request.Request.Items.Count <= 0)
        {
            throw new InvalidOrderException("Order must contain at least one item");
        }
        
        var hasDuplicates = request.Request.Items
            .GroupBy(x => x.ProductId)
            .Any(g => g.Count() > 1);
        
        if (hasDuplicates)
        {
            throw new DuplicateProductException();
        }

        var customer = await _dbContext.Customers.FindAsync(request.Request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.Request.CustomerId);
            throw new CustomerNotFoundException(request.Request.CustomerId);
        }

        var employee = await _dbContext.Employees.FindAsync(request.Request.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.Request.EmployeeId);
            throw new EmployeeNotFoundException(request.Request.EmployeeId);
        }
        
        try
        {
            _logger.LogInformation("Creating order using Customer ID: {CustomerId}", request.Request.CustomerId);
            var order = new Order
            {
                CustomerId = request.Request.CustomerId,
                EmployeeId = request.Request.EmployeeId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };
                    
            foreach (var item in request.Request.Items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found while creating order", item.ProductId);
                    throw new ProductNotFoundException(item.ProductId);
                }
            
                if (item.Quantity <= 0)
                {
                    _logger.LogWarning("Invalid quantity for product {ProductId}", item.ProductId);
                    throw new InvalidQuantityException(item.ProductId);
                }
            
                if (item.Quantity > product.Stock)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Stock}, Requested: {Quantity}",
                        item.ProductId, product.Stock, item.Quantity);
                    throw new InsufficientStockException(item.ProductId, product.Stock, item.Quantity);
                }
                
                var orderItems = new OrderItem()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                order.OrderItems.Add(orderItems);
            
                _logger.LogInformation("Adding product {ProductId} with quantity {Quantity} to order",
                    orderItems.ProductId, orderItems.Quantity);
            }
            _logger.LogInformation("Order contains {ItemCount} items", order.OrderItems.Count);
            
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
                    
            _logger.LogInformation("Order created successfully with ID: {OrderId}", order.Id);
            
            
            var response = new OrderResponse
            {
                Id = order.Id,
                CustomerId = request.Request.CustomerId,
                EmployeeId = request.Request.EmployeeId,
                CreatedAt = order.CreatedAt,
                Status = OrderStatus.Pending,
                Items = order.OrderItems.Select(oi=> new OrderItemResponse()
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalPrice = oi.Quantity * oi.Price
                }).ToList()
            };
            return response;
        }
        
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while creating order for Customer ID: {CustomerId}",
                request.Request.CustomerId);
            throw;
        }
    }
}
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository,
        IProductRepository productRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<OrderResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching orders. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new List<OrderResponse>();
        }
        
        var orders = await _orderRepository.GetAllAsync(pageNumber,pageSize);
        return orders.Select(o => new OrderResponse()
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CreatedAt = o.CreatedAt,
            Items = o.OrderItems.Select(i => new OrderItemResponse()
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            })

        }).ToList();
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        _logger.LogInformation("Creating order using Customer ID: {CustomerId}", request.CustomerId);
        
        var order = new Order
        {
            CustomerId = request.CustomerId,
            CreatedAt = DateTime.UtcNow,
            OrderItems = new List<OrderItem>()
        };
        
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found while creating order", item.ProductId);
                return null;
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

        var created = await _orderRepository.CreateAsync(order);
        
        _logger.LogInformation("Order created successfully with ID: {OrderId}", created.Id);

        var response = new OrderResponse
        {
            Id = created.Id,
            CustomerId = created.CustomerId,
            CreatedAt = created.CreatedAt,
            Items = created.OrderItems.Select(oi=> new OrderItemResponse()
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        };
        return response;
    }
    
    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", id);
            return null;
        }

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }
}
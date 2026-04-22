using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
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
                return null;
            }
            var orderItems = new OrderItem()
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price
            };
            order.OrderItems.Add(orderItems);
        }
        

        var created = await _orderRepository.CreateAsync(order);

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
            return null;

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
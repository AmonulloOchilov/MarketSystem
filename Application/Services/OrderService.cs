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

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _orderRepository.CreateAsync(order);

        return new OrderResponse
        {
            Id = created.Id,
            CustomerId = created.CustomerId,
            CreatedAt = created.CreatedAt,
            Items = new List<OrderItemResponse>()
        };
    }

    public async Task AddItemAsync(int orderId, AddOrderItemRequest request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);

        var item = new OrderItem
        {
            OrderId = orderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Price = product.Price
        };

        await _orderRepository.AddItemAsync(item);
    }

    public async Task<OrderResponse?> GetOrderAsync(int id)
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
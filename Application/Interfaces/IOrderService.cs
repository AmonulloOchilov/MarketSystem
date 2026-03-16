using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);

    Task AddItemAsync(int orderId, AddOrderItemRequest request);

    Task<OrderResponse?> GetOrderAsync(int id);
}
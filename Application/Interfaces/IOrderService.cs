using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponse>> GetAllAsync();
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetByIdAsync(int id);
}
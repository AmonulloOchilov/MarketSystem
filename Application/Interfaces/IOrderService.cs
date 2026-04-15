using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponse>> GetAllAsync(int pageNumber, int pageSize);
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetByIdAsync(int id);
}
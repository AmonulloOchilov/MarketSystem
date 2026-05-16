using Domain.Enums;

namespace Application.DTOs.Response;

public class OrderResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public IEnumerable<OrderItemResponse> Items { get; set; }
}
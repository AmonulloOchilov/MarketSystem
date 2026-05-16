namespace Application.DTOs.Response;

public class OrderResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<OrderItemResponse> Items { get; set; }
}
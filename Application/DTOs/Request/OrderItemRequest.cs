namespace Application.DTOs.Request;

public class OrderItemRequest
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}
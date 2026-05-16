namespace Application.DTOs.Response;

public class OrderItemResponse
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}
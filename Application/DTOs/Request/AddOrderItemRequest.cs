namespace Application.DTOs.Request;

public class AddOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
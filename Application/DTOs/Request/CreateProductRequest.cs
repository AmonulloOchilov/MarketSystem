namespace Application.DTOs.Request;

public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
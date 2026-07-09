namespace Application.DTOs.Response;

public class LowSellingProductsResponse
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public decimal SoldQuantity { get; set; }
    public decimal TotalRevenue { get; set; }
}
namespace Application.DTOs.Response;

public class TotalRevenueResponse
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
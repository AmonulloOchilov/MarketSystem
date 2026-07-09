namespace Application.DTOs.Response;

public class EmployeePerformanceResponse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal TotalRevenueGenerated { get; set; }
}
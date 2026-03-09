namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Quantity { get; set; }
    public DateOnly ExpireDate { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
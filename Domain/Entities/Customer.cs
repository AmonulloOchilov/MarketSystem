namespace Domain.Entities;

public class Customer : Person
{
    public string? PhoneNumber { get; set; }
    public List<Order> Orders { get; set; } = new();
}
namespace Domain.Entities;

public class Customer : Person
{
    public List<Order> Orders { get; set; } = new();
}
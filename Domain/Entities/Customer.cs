namespace Domain.Entities;

public class Customer : Person
{
    public string? PhoneNumber { get; set; }
    private List<Order> Orders { get; set; }
}
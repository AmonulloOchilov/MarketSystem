namespace Domain.Entities;

public class Employee : Person
{
    public string Position { get; set; } = null!;
    public ICollection<Order> Orders { get; set; }
}
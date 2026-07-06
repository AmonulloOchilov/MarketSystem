namespace Domain.Entities;

public class Employee : Person
{
    public string Position { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}
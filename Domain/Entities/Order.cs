using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CustomerId { get; set; }
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    public Customer Customer { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
namespace Application.DTOs.Request;

public class UpdateCustomerRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;
}
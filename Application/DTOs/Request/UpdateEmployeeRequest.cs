namespace Application.DTOs.Request;

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; }
    public string Role { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Position { get; set; } = null!;
}
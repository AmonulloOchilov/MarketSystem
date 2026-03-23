namespace Application.DTOs.Request;

public class UpdateAdminRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? Email { get; set; }
}
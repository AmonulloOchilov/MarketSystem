namespace Application.DTOs.Request;

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Position { get; set; } = null!;
}
namespace Application.DTOs.Response;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; }
    public string Role { get; set; }
    public string Position { get; set; } = null!;
    public string? Email { get; set; }
}
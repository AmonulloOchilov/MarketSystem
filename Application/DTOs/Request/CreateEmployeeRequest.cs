namespace Application.DTOs.Request;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string? Email { get; set; }
}
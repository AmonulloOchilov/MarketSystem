namespace Application.DTOs.Request;

public class UpdateAdminRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
}
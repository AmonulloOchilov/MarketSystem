namespace Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}
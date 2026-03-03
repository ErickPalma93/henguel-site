namespace HenguelSistemas.Domain.Entities;

public class WaitlistEntry
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public bool ConsentGiven { get; set; } = false;
    public DateTime? ConsentDate { get; set; }
}
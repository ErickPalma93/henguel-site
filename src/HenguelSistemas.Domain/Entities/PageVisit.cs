namespace HenguelSistemas.Domain.Entities;

public class PageVisit
{
    public int Id { get; set; }
    public string Page { get; set; } = string.Empty;
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    public string? HashedIp { get; set; }
    public string? UserAgent { get; set; }
}
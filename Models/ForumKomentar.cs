namespace SipintarBeltim.Models;

public class ForumKomentar
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ForumTopikId { get; set; }
    public string Isi { get; set; } = string.Empty;

    // Meta
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ForumTopik ForumTopik { get; set; } = null!;
}

namespace SipintarBeltim.Models;

public class ForumPengumuman
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
    public bool IsActive { get; set; } = true;

    // Meta
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

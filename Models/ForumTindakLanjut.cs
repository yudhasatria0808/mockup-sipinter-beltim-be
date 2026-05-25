namespace SipintarBeltim.Models;

public class ForumTindakLanjut
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ForumArahanId { get; set; }
    public string Isi { get; set; } = string.Empty;
    public string Status { get; set; } = "belum"; // belum, sedang, selesai
    public DateTime? TanggalSelesai { get; set; }

    // Meta
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ForumArahan ForumArahan { get; set; } = null!;
}

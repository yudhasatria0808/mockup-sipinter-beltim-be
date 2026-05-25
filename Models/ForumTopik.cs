namespace SipintarBeltim.Models;

public class ForumTopik
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Kategori { get; set; } = string.Empty; // keamanan, konflik, koordinasi, lainnya
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
    public string Status { get; set; } = "aktif"; // aktif, ditutup, diarsipkan

    // Relasi ke data existing (opsional)
    public Guid? KewaspadaanDiniId { get; set; }
    public Guid? PeristiwaKonflikId { get; set; }

    // Meta
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<ForumKomentar> Komentars { get; set; } = new List<ForumKomentar>();
}

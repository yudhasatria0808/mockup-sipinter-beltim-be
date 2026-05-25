namespace SipintarBeltim.Models;

public class ForumArahan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
    public string InstansiTujuan { get; set; } = string.Empty; // instansi yang dituju

    // Relasi ke topik (opsional)
    public Guid? ForumTopikId { get; set; }

    // Meta
    public Guid CreatedByUserId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ForumTopik? ForumTopik { get; set; }
    public ICollection<ForumTindakLanjut> TindakLanjuts { get; set; } = new List<ForumTindakLanjut>();
}

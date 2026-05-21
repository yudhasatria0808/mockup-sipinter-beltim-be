namespace SipintarBeltim.Models;

public class LevelDampak
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

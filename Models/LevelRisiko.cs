namespace SipintarBeltim.Models;

public class LevelRisiko
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nama { get; set; } = string.Empty;
    public string Warna { get; set; } = string.Empty; // hex color
    public int SkorMin { get; set; }
    public int SkorMax { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

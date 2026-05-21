namespace SipintarBeltim.Models;

public class Instansi
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

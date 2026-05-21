namespace SipintarBeltim.Models;

public class Pelanggan
{
    public int Id { get; set; }
    public string Nama { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string NomorSambungan { get; set; } = string.Empty;
    public string? NoTelp { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

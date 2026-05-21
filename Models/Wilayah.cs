namespace SipintarBeltim.Models;

public class Wilayah
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tipe { get; set; } = string.Empty; // Provinsi, Kabupaten, Kota, Kecamatan, Kelurahan, Desa
    public string Nama { get; set; } = string.Empty;
    public string KodeBps { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Wilayah? Parent { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Wilayah> Children { get; set; } = new List<Wilayah>();
}

namespace SipintarBeltim.Models;

public class KewaspadaanDini
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Periode { get; set; }
    public string Aspek { get; set; } = string.Empty;

    // Wilayah / Lokasi
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? BuktiFoto { get; set; }
    public string SumberInformasi { get; set; } = string.Empty;

    // Analisis Risiko
    public string KemungkinanAncamanLevel { get; set; } = string.Empty;
    public string KemungkinanAncamanDeskripsi { get; set; } = string.Empty;
    public string? Hambatan { get; set; }
    public string? Tantangan { get; set; }
    public string? Gangguan { get; set; }
    public string PrediksiDampakLevel { get; set; } = string.Empty;
    public string PrediksiDampakDeskripsi { get; set; } = string.Empty;
    public string Rekomendasi { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty; // Rendah, Sedang, Tinggi, Sangat Tinggi

    // Approval
    public string Status { get; set; } = "draft"; // draft, menunggu, disetujui, ditolak
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Meta
    public Guid? CreatedByUserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

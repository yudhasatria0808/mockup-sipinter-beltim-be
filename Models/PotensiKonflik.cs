namespace SipintarBeltim.Models;

public class PotensiKonflik
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
    public string? SumberInformasi { get; set; }

    // Analisis Konflik
    public string NamaPotensiKonflik { get; set; } = string.Empty;
    public string KemungkinanLevel { get; set; } = string.Empty;
    public string? KemungkinanDeskripsi { get; set; }
    public string? SumberSebabPermasalahan { get; set; }
    public string? LatarBelakangMasalah { get; set; }
    public string DampakLevel { get; set; } = string.Empty;
    public string? DampakDeskripsi { get; set; }
    public string? UpayaPenanganan { get; set; }
    public string? KeteranganDetail { get; set; }
    public string Rekomendasi { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;

    // Approval
    public string Status { get; set; } = "draft";
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedByRole { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Meta
    public Guid? CreatedByUserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

namespace SipintarBeltim.Models;

public class PeristiwaKonflik
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Periode { get; set; }

    // Identitas Peristiwa
    public string NamaPeristiwa { get; set; } = string.Empty;
    public string? SumberSebabKonflik { get; set; }
    public string? LatarBelakangKejadian { get; set; }
    public string? DeskripsiAkibatPeristiwa { get; set; }

    // Korban & Kerugian
    public int KorbanKritis { get; set; }
    public int KorbanLukaLuka { get; set; }
    public int KorbanMengungsi { get; set; }
    public decimal KerugianMateril { get; set; }

    // Upaya
    public string? UpayaPenanganan { get; set; }
    public string? UpayaPemulihan { get; set; }

    // Wilayah / Lokasi
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }

    // Bukti & Sumber
    public string? BuktiFoto { get; set; }
    public string? SumberInformasi { get; set; }
    public string? Keterangan { get; set; }
    public string? SaranTindakLanjut { get; set; }

    // Tingkat Risiko
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

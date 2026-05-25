namespace SipintarBeltim.Models;

public class WargaNegaraAsing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Periode { get; set; }

    // Identitas
    public string JenisKelamin { get; set; } = string.Empty;
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string NoPaspor { get; set; } = string.Empty;

    // Visa / Izin Tinggal
    public string JenisVisa { get; set; } = string.Empty;
    public DateTime MasaBerlakuVisa { get; set; }
    public string? Pekerjaan { get; set; }
    public string? Sponsor { get; set; }

    // Alamat Domisili
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }

    // Lama Tinggal & Status
    public string? LamaTinggal { get; set; }
    public string StatusTinggal { get; set; } = "Aktif";
    public string? Keterangan { get; set; }

    // Sumber
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }

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

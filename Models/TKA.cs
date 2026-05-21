namespace SipintarBeltim.Models;

public class TenagaKerjaAsing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Periode { get; set; }

    // Identitas TKA
    public string NamaTKA { get; set; } = string.Empty;
    public string JenisKelamin { get; set; } = string.Empty;
    public string NamaPerusahaan { get; set; } = string.Empty;
    public string? JabatanKeterampilan { get; set; }
    public string? NoTelepon { get; set; }
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string NoPaspor { get; set; } = string.Empty;

    // IMTA / RPTKA
    public string? NomorIMTA { get; set; }
    public DateTime? TanggalMulaiIMTA { get; set; }
    public DateTime? TanggalBerakhirIMTA { get; set; }

    // Jenis Izin Tinggal
    public string JenisIzinTinggal { get; set; } = string.Empty;

    // Alamat Domisili
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }

    // Keterangan & Sumber
    public string? Keterangan { get; set; }
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }

    // Approval
    public string Status { get; set; } = "draft";
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Meta
    public Guid? CreatedByUserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

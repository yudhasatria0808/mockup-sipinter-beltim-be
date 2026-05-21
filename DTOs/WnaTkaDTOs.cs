namespace SipintarBeltim.DTOs;

// ===== WNA =====
public class WnaListDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string JenisKelamin { get; set; } = string.Empty;
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string NoPaspor { get; set; } = string.Empty;
    public string JenisVisa { get; set; } = string.Empty;
    public string MasaBerlakuVisa { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string StatusTinggal { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class WnaDetailDto : WnaListDto
{
    public string? Pekerjaan { get; set; }
    public string? Sponsor { get; set; }
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? LamaTinggal { get; set; }
    public string? Keterangan { get; set; }
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedAt { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class WnaCreateRequest
{
    public string Periode { get; set; } = string.Empty;
    public string JenisKelamin { get; set; } = string.Empty;
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string NoPaspor { get; set; } = string.Empty;
    public string JenisVisa { get; set; } = string.Empty;
    public string MasaBerlakuVisa { get; set; } = string.Empty;
    public string? Pekerjaan { get; set; }
    public string? Sponsor { get; set; }
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? LamaTinggal { get; set; }
    public string StatusTinggal { get; set; } = "Aktif";
    public string? Keterangan { get; set; }
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string Status { get; set; } = "draft";
}

public class WnaUpdateRequest : WnaCreateRequest { }

// ===== TKA =====
public class TkaListDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string NamaTKA { get; set; } = string.Empty;
    public string JenisKelamin { get; set; } = string.Empty;
    public string NamaPerusahaan { get; set; } = string.Empty;
    public string? JabatanKeterampilan { get; set; }
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string JenisIzinTinggal { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}

public class TkaDetailDto : TkaListDto
{
    public string NoPaspor { get; set; } = string.Empty;
    public string? NoTelepon { get; set; }
    public string? NomorIMTA { get; set; }
    public string? TanggalMulaiIMTA { get; set; }
    public string? TanggalBerakhirIMTA { get; set; }
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? Keterangan { get; set; }
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedAt { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class TkaCreateRequest
{
    public string Periode { get; set; } = string.Empty;
    public string NamaTKA { get; set; } = string.Empty;
    public string JenisKelamin { get; set; } = string.Empty;
    public string NamaPerusahaan { get; set; } = string.Empty;
    public string? JabatanKeterampilan { get; set; }
    public string? NoTelepon { get; set; }
    public string Kewarganegaraan { get; set; } = string.Empty;
    public string NoPaspor { get; set; } = string.Empty;
    public string? NomorIMTA { get; set; }
    public string? TanggalMulaiIMTA { get; set; }
    public string? TanggalBerakhirIMTA { get; set; }
    public string JenisIzinTinggal { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? Keterangan { get; set; }
    public string? SumberInformasi { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string Status { get; set; } = "draft";
}

public class TkaUpdateRequest : TkaCreateRequest { }

namespace SipintarBeltim.DTOs;

public class PotensiKonflikListDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;
    public string NamaPotensiKonflik { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class PotensiKonflikDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? BuktiFoto { get; set; }
    public string? SumberInformasi { get; set; }
    public string NamaPotensiKonflik { get; set; } = string.Empty;
    public LevelDeskripsiDto KemungkinanPotensiKonflik { get; set; } = new();
    public string? SumberSebabPermasalahan { get; set; }
    public string? LatarBelakangMasalah { get; set; }
    public LevelDeskripsiDto DampakPotensiKonflik { get; set; } = new();
    public string? UpayaPenanganan { get; set; }
    public string? KeteranganDetail { get; set; }
    public string Rekomendasi { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class LevelDeskripsiDto
{
    public string Level { get; set; } = string.Empty;
    public string Deskripsi { get; set; } = string.Empty;
}

public class PotensiKonflikCreateRequest
{
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? SumberInformasi { get; set; }
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
    public string Status { get; set; } = "draft";
}

public class PotensiKonflikUpdateRequest : PotensiKonflikCreateRequest { }

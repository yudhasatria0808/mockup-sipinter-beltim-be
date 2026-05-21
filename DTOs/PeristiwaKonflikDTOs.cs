namespace SipintarBeltim.DTOs;

public class PeristiwaKonflikListDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string NamaPeristiwa { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public int KorbanKritis { get; set; }
    public int KorbanLukaLuka { get; set; }
    public int KorbanMengungsi { get; set; }
    public decimal KerugianMateril { get; set; }
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class PeristiwaKonflikDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string NamaPeristiwa { get; set; } = string.Empty;
    public string? SumberSebabKonflik { get; set; }
    public string? LatarBelakangKejadian { get; set; }
    public string? DeskripsiAkibatPeristiwa { get; set; }
    public int KorbanKritis { get; set; }
    public int KorbanLukaLuka { get; set; }
    public int KorbanMengungsi { get; set; }
    public decimal KerugianMateril { get; set; }
    public string? UpayaPenanganan { get; set; }
    public string? UpayaPemulihan { get; set; }
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? BuktiFoto { get; set; }
    public string? SumberInformasi { get; set; }
    public string? Keterangan { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class PeristiwaKonflikCreateRequest
{
    public string Periode { get; set; } = string.Empty;
    public string NamaPeristiwa { get; set; } = string.Empty;
    public string? SumberSebabKonflik { get; set; }
    public string? LatarBelakangKejadian { get; set; }
    public string? DeskripsiAkibatPeristiwa { get; set; }
    public int KorbanKritis { get; set; }
    public int KorbanLukaLuka { get; set; }
    public int KorbanMengungsi { get; set; }
    public decimal KerugianMateril { get; set; }
    public string? UpayaPenanganan { get; set; }
    public string? UpayaPemulihan { get; set; }
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? SumberInformasi { get; set; }
    public string? Keterangan { get; set; }
    public string? SaranTindakLanjut { get; set; }
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
}

public class PeristiwaKonflikUpdateRequest : PeristiwaKonflikCreateRequest { }

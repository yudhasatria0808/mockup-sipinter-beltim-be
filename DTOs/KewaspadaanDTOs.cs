namespace SipintarBeltim.DTOs;

public class KewaspadaanListDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class KewaspadaanDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;

    // Wilayah
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string? BuktiFoto { get; set; }
    public string SumberInformasi { get; set; } = string.Empty;

    // Analisis Risiko
    public KemungkinanAncamanDto KemungkinanAncaman { get; set; } = new();
    public string? Hambatan { get; set; }
    public string? Tantangan { get; set; }
    public string? Gangguan { get; set; }
    public PrediksiDampakDto PrediksiDampak { get; set; } = new();
    public string Rekomendasi { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;

    // Approval
    public string Status { get; set; } = string.Empty;
    public string? CatatanApproval { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ApprovedAt { get; set; }

    // Meta
    public string CreatedBy { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class KemungkinanAncamanDto
{
    public string Level { get; set; } = string.Empty;
    public string Deskripsi { get; set; } = string.Empty;
}

public class PrediksiDampakDto
{
    public string Level { get; set; } = string.Empty;
    public string Deskripsi { get; set; } = string.Empty;
}

public class KewaspadaanCreateRequest
{
    public string Periode { get; set; } = string.Empty;
    public string Aspek { get; set; } = string.Empty;
    public string Kabupaten { get; set; } = string.Empty;
    public string Kecamatan { get; set; } = string.Empty;
    public string Desa { get; set; } = string.Empty;
    public string? AlamatDetail { get; set; }
    public string? TitikKoordinat { get; set; }
    public string SumberInformasi { get; set; } = string.Empty;
    public string KemungkinanLevel { get; set; } = string.Empty;
    public string KemungkinanDeskripsi { get; set; } = string.Empty;
    public string? Hambatan { get; set; }
    public string? Tantangan { get; set; }
    public string? Gangguan { get; set; }
    public string DampakLevel { get; set; } = string.Empty;
    public string DampakDeskripsi { get; set; } = string.Empty;
    public string Rekomendasi { get; set; } = string.Empty;
    public string TingkatRisiko { get; set; } = string.Empty;
    public string Status { get; set; } = "draft"; // draft or menunggu
}

public class KewaspadaanUpdateRequest : KewaspadaanCreateRequest { }

public class KewaspadaanApprovalRequest
{
    public string Action { get; set; } = string.Empty; // disetujui or ditolak
    public string? Catatan { get; set; }
}

namespace SipintarBeltim.DTOs;

// ===== ASPEK =====
public class AspekDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class AspekCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class AspekUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

// ===== JENIS KONFLIK =====
public class JenisKonflikDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class JenisKonflikCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class JenisKonflikUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

// ===== INSTANSI =====
public class InstansiDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class InstansiCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

public class InstansiUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
}

// ===== WILAYAH =====
public class WilayahDto
{
    public string Id { get; set; } = string.Empty;
    public string Tipe { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string KodeBps { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ParentNama { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class WilayahCreateRequest
{
    public string Tipe { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string KodeBps { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class WilayahUpdateRequest
{
    public string Tipe { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string KodeBps { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

// ===== LEVEL KEMUNGKINAN =====
public class LevelKemungkinanDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

public class LevelKemungkinanCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

public class LevelKemungkinanUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

// ===== LEVEL DAMPAK =====
public class LevelDampakDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

public class LevelDampakCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

public class LevelDampakUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public int Skor { get; set; }
    public string? Deskripsi { get; set; }
}

// ===== LEVEL RISIKO =====
public class LevelRisikoDto
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Warna { get; set; } = string.Empty;
    public int SkorMin { get; set; }
    public int SkorMax { get; set; }
}

public class LevelRisikoCreateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string Warna { get; set; } = string.Empty;
    public int SkorMin { get; set; }
    public int SkorMax { get; set; }
}

public class LevelRisikoUpdateRequest
{
    public string Nama { get; set; } = string.Empty;
    public string Warna { get; set; } = string.Empty;
    public int SkorMin { get; set; }
    public int SkorMax { get; set; }
}

// ===== MATRIKS RISIKO =====
public class MatriksRisikoDto
{
    public string Id { get; set; } = string.Empty;
    public string KemungkinanId { get; set; } = string.Empty;
    public string KemungkinanNama { get; set; } = string.Empty;
    public int KemungkinanSkor { get; set; }
    public string DampakId { get; set; } = string.Empty;
    public string DampakNama { get; set; } = string.Empty;
    public int DampakSkor { get; set; }
    public string LevelRisikoId { get; set; } = string.Empty;
    public string LevelRisikoNama { get; set; } = string.Empty;
    public string LevelRisikoWarna { get; set; } = string.Empty;
}

public class MatriksRisikoCreateRequest
{
    public string KemungkinanId { get; set; } = string.Empty;
    public string DampakId { get; set; } = string.Empty;
    public string LevelRisikoId { get; set; } = string.Empty;
}

public class MatriksRisikoUpdateRequest
{
    public string KemungkinanId { get; set; } = string.Empty;
    public string DampakId { get; set; } = string.Empty;
    public string LevelRisikoId { get; set; } = string.Empty;
}

public class MatriksRisikoBulkRequest
{
    public List<MatriksRisikoCreateRequest> Items { get; set; } = new();
}

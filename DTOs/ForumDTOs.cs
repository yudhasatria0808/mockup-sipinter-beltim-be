namespace SipintarBeltim.DTOs;

// ===== FORUM TOPIK =====

public class ForumTopikListDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Kategori { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int JumlahKomentar { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string? LastActivityAt { get; set; }
}

public class ForumTopikDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Kategori { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? KewaspadaanDiniId { get; set; }
    public string? PeristiwaKonflikId { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public List<ForumKomentarDto> Komentars { get; set; } = new();
}

public class ForumTopikCreateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Kategori { get; set; } = "lainnya"; // keamanan, konflik, koordinasi, lainnya
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
    public string? KewaspadaanDiniId { get; set; }
    public string? PeristiwaKonflikId { get; set; }
}

public class ForumTopikUpdateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Kategori { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

// ===== FORUM KOMENTAR =====

public class ForumKomentarDto
{
    public string Id { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ForumKomentarCreateRequest
{
    public string Isi { get; set; } = string.Empty;
}

// ===== FORUM ARAHAN =====

public class ForumArahanListDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string InstansiTujuan { get; set; } = string.Empty;
    public string? ForumTopikId { get; set; }
    public string? ForumTopikJudul { get; set; }
    public int JumlahTindakLanjut { get; set; }
    public int TindakLanjutSelesai { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ForumArahanDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string InstansiTujuan { get; set; } = string.Empty;
    public string? ForumTopikId { get; set; }
    public string? ForumTopikJudul { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public List<ForumTindakLanjutDto> TindakLanjuts { get; set; } = new();
}

public class ForumArahanCreateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
    public string InstansiTujuan { get; set; } = string.Empty;
    public string? ForumTopikId { get; set; }
}

public class ForumArahanUpdateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public string InstansiTujuan { get; set; } = string.Empty;
}

// ===== FORUM TINDAK LANJUT =====

public class ForumTindakLanjutDto
{
    public string Id { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TanggalSelesai { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ForumTindakLanjutCreateRequest
{
    public string Isi { get; set; } = string.Empty;
}

public class ForumTindakLanjutUpdateStatusRequest
{
    public string Status { get; set; } = string.Empty; // belum, sedang, selesai
}

// ===== FORUM PENGUMUMAN =====

public class ForumPengumumanListDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ForumPengumumanDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByRole { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class ForumPengumumanCreateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = "biasa"; // biasa, penting, urgent
}

public class ForumPengumumanUpdateRequest
{
    public string Judul { get; set; } = string.Empty;
    public string Isi { get; set; } = string.Empty;
    public string Prioritas { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

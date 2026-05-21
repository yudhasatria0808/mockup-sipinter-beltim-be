using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class PotensiKonflikEndpoints
{
    public static void MapPotensiKonflikEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/potensi-konflik")
            .WithTags("Potensi Konflik").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, string? aspek,
            string? status, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.PotensiKonfliks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(aspek))
                query = query.Where(p => p.Aspek == aspek);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);
            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var s = generalSearch.ToLower();
                query = query.Where(p =>
                    p.NamaPotensiKonflik.ToLower().Contains(s) ||
                    p.Kabupaten.ToLower().Contains(s) ||
                    p.Kecamatan.ToLower().Contains(s) ||
                    p.Desa.ToLower().Contains(s));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);
            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .Select(p => new PotensiKonflikListDto
                {
                    Id = p.Id.ToString(), Periode = p.Periode.ToString("yyyy-MM-dd"),
                    Aspek = p.Aspek, NamaPotensiKonflik = p.NamaPotensiKonflik,
                    Kabupaten = p.Kabupaten, Kecamatan = p.Kecamatan, Desa = p.Desa,
                    TingkatRisiko = p.TingkatRisiko, Status = p.Status,
                    CreatedBy = p.CreatedBy, CreatedAt = p.CreatedAt.ToString("o")
                }).ToListAsync();

            return Results.Ok(new PaginatedResponse<PotensiKonflikListDto>
            { Data = items, CurrentPage = page, PageSize = size, TotalPages = totalPages, TotalCount = totalCount });
        }).WithName("GetPotensiKonflik").WithOpenApi();

        // GET EWS (only disetujui)
        group.MapGet("/ews", async (string? aspek, string? tingkatRisiko, AppDbContext db) =>
        {
            var query = db.PotensiKonfliks.Where(p => p.Status == "disetujui").AsQueryable();
            if (!string.IsNullOrWhiteSpace(aspek)) query = query.Where(p => p.Aspek == aspek);
            if (!string.IsNullOrWhiteSpace(tingkatRisiko)) query = query.Where(p => p.TingkatRisiko == tingkatRisiko);

            var items = await query.OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDetail(p)).ToListAsync();

            var allApproved = await db.PotensiKonfliks.Where(p => p.Status == "disetujui").ToListAsync();
            var stats = new {
                total = allApproved.Count,
                sangatTinggi = allApproved.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                tinggi = allApproved.Count(p => p.TingkatRisiko == "Tinggi"),
                sedang = allApproved.Count(p => p.TingkatRisiko == "Sedang"),
                rendah = allApproved.Count(p => p.TingkatRisiko == "Rendah"),
            };
            return Results.Ok(new ApiResponse<object> { Success = true, Data = new { stats, items } });
        }).WithName("GetPotensiKonflikEWS").WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.PotensiKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            return Results.Ok(new ApiResponse<PotensiKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("GetPotensiKonflikById").WithOpenApi();

        // POST create
        group.MapPost("/", async (PotensiKonflikCreateRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var userName = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var item = new PotensiKonflik
            {
                Periode = DateTime.Parse(req.Periode), Aspek = req.Aspek,
                Kabupaten = req.Kabupaten, Kecamatan = req.Kecamatan, Desa = req.Desa,
                AlamatDetail = req.AlamatDetail, TitikKoordinat = req.TitikKoordinat,
                SumberInformasi = req.SumberInformasi,
                NamaPotensiKonflik = req.NamaPotensiKonflik,
                KemungkinanLevel = req.KemungkinanLevel, KemungkinanDeskripsi = req.KemungkinanDeskripsi,
                SumberSebabPermasalahan = req.SumberSebabPermasalahan,
                LatarBelakangMasalah = req.LatarBelakangMasalah,
                DampakLevel = req.DampakLevel, DampakDeskripsi = req.DampakDeskripsi,
                UpayaPenanganan = req.UpayaPenanganan, KeteranganDetail = req.KeteranganDetail,
                Rekomendasi = req.Rekomendasi, TingkatRisiko = req.TingkatRisiko,
                Status = req.Status, CreatedBy = userName,
                CreatedByUserId = userId != null ? Guid.Parse(userId) : null,
            };
            db.PotensiKonfliks.Add(item);
            await db.SaveChangesAsync();
            return Results.Created($"/api/potensi-konflik/{item.Id}",
                new ApiResponse<PotensiKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("CreatePotensiKonflik").WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, PotensiKonflikUpdateRequest req, AppDbContext db) =>
        {
            var item = await db.PotensiKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat diubah" });

            item.Periode = DateTime.Parse(req.Periode); item.Aspek = req.Aspek;
            item.Kabupaten = req.Kabupaten; item.Kecamatan = req.Kecamatan; item.Desa = req.Desa;
            item.AlamatDetail = req.AlamatDetail; item.TitikKoordinat = req.TitikKoordinat;
            item.SumberInformasi = req.SumberInformasi;
            item.NamaPotensiKonflik = req.NamaPotensiKonflik;
            item.KemungkinanLevel = req.KemungkinanLevel; item.KemungkinanDeskripsi = req.KemungkinanDeskripsi;
            item.SumberSebabPermasalahan = req.SumberSebabPermasalahan;
            item.LatarBelakangMasalah = req.LatarBelakangMasalah;
            item.DampakLevel = req.DampakLevel; item.DampakDeskripsi = req.DampakDeskripsi;
            item.UpayaPenanganan = req.UpayaPenanganan; item.KeteranganDetail = req.KeteranganDetail;
            item.Rekomendasi = req.Rekomendasi; item.TingkatRisiko = req.TingkatRisiko;
            item.Status = req.Status; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<PotensiKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("UpdatePotensiKonflik").WithOpenApi();

        // POST approval
        group.MapPost("/{id}/approval", async (string id, KewaspadaanApprovalRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var item = await db.PotensiKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status != "menunggu") return Results.BadRequest(new { success = false, message = "Hanya data 'menunggu' yang dapat diproses" });
            if (req.Action != "disetujui" && req.Action != "ditolak") return Results.BadRequest(new { success = false, message = "Action harus 'disetujui' atau 'ditolak'" });

            var userName = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            item.Status = req.Action; item.CatatanApproval = req.Catatan;
            item.ApprovedBy = userName; item.ApprovedAt = DateTime.UtcNow; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<PotensiKonflikDetailDto> { Success = true, Data = MapToDetail(item),
                Message = req.Action == "disetujui" ? "Data berhasil disetujui" : "Data berhasil ditolak" });
        }).WithName("ApprovePotensiKonflik").WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.PotensiKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat dihapus" });
            db.PotensiKonfliks.Remove(item); await db.SaveChangesAsync();
            return Results.Ok(new { success = true, message = "Data berhasil dihapus" });
        }).WithName("DeletePotensiKonflik").WithOpenApi();
    }

    private static PotensiKonflikDetailDto MapToDetail(PotensiKonflik p) => new()
    {
        Id = p.Id.ToString(), Periode = p.Periode.ToString("yyyy-MM-dd"), Aspek = p.Aspek,
        Kabupaten = p.Kabupaten, Kecamatan = p.Kecamatan, Desa = p.Desa,
        AlamatDetail = p.AlamatDetail, TitikKoordinat = p.TitikKoordinat,
        BuktiFoto = p.BuktiFoto, SumberInformasi = p.SumberInformasi,
        NamaPotensiKonflik = p.NamaPotensiKonflik,
        KemungkinanPotensiKonflik = new LevelDeskripsiDto { Level = p.KemungkinanLevel, Deskripsi = p.KemungkinanDeskripsi ?? "" },
        SumberSebabPermasalahan = p.SumberSebabPermasalahan, LatarBelakangMasalah = p.LatarBelakangMasalah,
        DampakPotensiKonflik = new LevelDeskripsiDto { Level = p.DampakLevel, Deskripsi = p.DampakDeskripsi ?? "" },
        UpayaPenanganan = p.UpayaPenanganan, KeteranganDetail = p.KeteranganDetail,
        Rekomendasi = p.Rekomendasi, TingkatRisiko = p.TingkatRisiko,
        Status = p.Status, CatatanApproval = p.CatatanApproval,
        ApprovedBy = p.ApprovedBy, ApprovedAt = p.ApprovedAt?.ToString("o"),
        CreatedBy = p.CreatedBy, CreatedAt = p.CreatedAt.ToString("o")
    };
}

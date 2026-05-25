using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class PeristiwaKonflikEndpoints
{
    public static void MapPeristiwaKonflikEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/peristiwa-konflik")
            .WithTags("Peristiwa Konflik").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, string? tingkatRisiko,
            string? status, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.PeristiwaKonfliks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tingkatRisiko))
                query = query.Where(p => p.TingkatRisiko == tingkatRisiko);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);
            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var s = generalSearch.ToLower();
                query = query.Where(p =>
                    p.NamaPeristiwa.ToLower().Contains(s) ||
                    p.Kabupaten.ToLower().Contains(s) ||
                    p.Kecamatan.ToLower().Contains(s) ||
                    p.Desa.ToLower().Contains(s));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);
            var items = await query.OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .Select(p => new PeristiwaKonflikListDto
                {
                    Id = p.Id.ToString(), Periode = p.Periode.ToString("yyyy-MM-dd"),
                    NamaPeristiwa = p.NamaPeristiwa,
                    Kabupaten = p.Kabupaten, Kecamatan = p.Kecamatan, Desa = p.Desa,
                    KorbanKritis = p.KorbanKritis, KorbanLukaLuka = p.KorbanLukaLuka,
                    KorbanMengungsi = p.KorbanMengungsi, KerugianMateril = p.KerugianMateril,
                    TingkatRisiko = p.TingkatRisiko, Status = p.Status,
                    CreatedBy = p.CreatedBy, CreatedAt = p.CreatedAt.ToString("o")
                }).ToListAsync();

            return Results.Ok(new PaginatedResponse<PeristiwaKonflikListDto>
            { Data = items, CurrentPage = page, PageSize = size, TotalPages = totalPages, TotalCount = totalCount });
        }).WithName("GetPeristiwaKonflik").WithOpenApi();

        // GET EWS (only disetujui)
        group.MapGet("/ews", async (string? tingkatRisiko, AppDbContext db) =>
        {
            var query = db.PeristiwaKonfliks.Where(p => p.Status == "disetujui").AsQueryable();
            if (!string.IsNullOrWhiteSpace(tingkatRisiko))
                query = query.Where(p => p.TingkatRisiko == tingkatRisiko);

            var items = await query.OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDetail(p)).ToListAsync();

            var allApproved = await db.PeristiwaKonfliks.Where(p => p.Status == "disetujui").ToListAsync();
            var stats = new
            {
                total = allApproved.Count,
                sangatTinggi = allApproved.Count(p => p.TingkatRisiko == "Sangat Tinggi"),
                tinggi = allApproved.Count(p => p.TingkatRisiko == "Tinggi"),
                sedang = allApproved.Count(p => p.TingkatRisiko == "Sedang"),
                rendah = allApproved.Count(p => p.TingkatRisiko == "Rendah"),
                totalKorbanKritis = allApproved.Sum(p => p.KorbanKritis),
                totalKorbanLuka = allApproved.Sum(p => p.KorbanLukaLuka),
                totalKorbanMengungsi = allApproved.Sum(p => p.KorbanMengungsi),
                totalKerugian = allApproved.Sum(p => p.KerugianMateril),
            };
            return Results.Ok(new ApiResponse<object> { Success = true, Data = new { stats, items } });
        }).WithName("GetPeristiwaKonflikEWS").WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.PeristiwaKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            return Results.Ok(new ApiResponse<PeristiwaKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("GetPeristiwaKonflikById").WithOpenApi();

        // POST create
        group.MapPost("/", async (PeristiwaKonflikCreateRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var userName = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = new PeristiwaKonflik
            {
                Periode = DateTime.Parse(req.Periode), NamaPeristiwa = req.NamaPeristiwa,
                SumberSebabKonflik = req.SumberSebabKonflik, LatarBelakangKejadian = req.LatarBelakangKejadian,
                DeskripsiAkibatPeristiwa = req.DeskripsiAkibatPeristiwa,
                KorbanKritis = req.KorbanKritis, KorbanLukaLuka = req.KorbanLukaLuka,
                KorbanMengungsi = req.KorbanMengungsi, KerugianMateril = req.KerugianMateril,
                UpayaPenanganan = req.UpayaPenanganan, UpayaPemulihan = req.UpayaPemulihan,
                Kabupaten = req.Kabupaten, Kecamatan = req.Kecamatan, Desa = req.Desa,
                AlamatDetail = req.AlamatDetail, TitikKoordinat = req.TitikKoordinat,
                SumberInformasi = req.SumberInformasi, Keterangan = req.Keterangan,
                SaranTindakLanjut = req.SaranTindakLanjut,
                TingkatRisiko = req.TingkatRisiko, Status = req.Status,
                CreatedBy = userName, CreatedByUserId = userId != null ? Guid.Parse(userId) : null,
            };
            db.PeristiwaKonfliks.Add(item); await db.SaveChangesAsync();
            return Results.Created($"/api/peristiwa-konflik/{item.Id}",
                new ApiResponse<PeristiwaKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("CreatePeristiwaKonflik").WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, PeristiwaKonflikUpdateRequest req, AppDbContext db) =>
        {
            var item = await db.PeristiwaKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat diubah" });

            item.Periode = DateTime.Parse(req.Periode); item.NamaPeristiwa = req.NamaPeristiwa;
            item.SumberSebabKonflik = req.SumberSebabKonflik; item.LatarBelakangKejadian = req.LatarBelakangKejadian;
            item.DeskripsiAkibatPeristiwa = req.DeskripsiAkibatPeristiwa;
            item.KorbanKritis = req.KorbanKritis; item.KorbanLukaLuka = req.KorbanLukaLuka;
            item.KorbanMengungsi = req.KorbanMengungsi; item.KerugianMateril = req.KerugianMateril;
            item.UpayaPenanganan = req.UpayaPenanganan; item.UpayaPemulihan = req.UpayaPemulihan;
            item.Kabupaten = req.Kabupaten; item.Kecamatan = req.Kecamatan; item.Desa = req.Desa;
            item.AlamatDetail = req.AlamatDetail; item.TitikKoordinat = req.TitikKoordinat;
            item.SumberInformasi = req.SumberInformasi; item.Keterangan = req.Keterangan;
            item.SaranTindakLanjut = req.SaranTindakLanjut;
            item.TingkatRisiko = req.TingkatRisiko; item.Status = req.Status; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<PeristiwaKonflikDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("UpdatePeristiwaKonflik").WithOpenApi();

        // POST approval
        group.MapPost("/{id}/approval", async (string id, KewaspadaanApprovalRequest req, HttpContext ctx, AppDbContext db) =>
        {
            // Check CanApprove permission
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Results.Unauthorized();
            var user = await db.Users.FindAsync(Guid.Parse(userId));
            if (user is null) return Results.Unauthorized();
            var menuId = Guid.Parse("b0000006-0000-0000-0000-000000000001"); // Form Peristiwa Konflik
            var hasPerm = await db.RolePermissions.AnyAsync(rp => rp.RoleId == user.RoleId && rp.MenuId == menuId && rp.CanApprove);
            if (!hasPerm)
                return Results.Json(new { success = false, message = "Anda tidak memiliki izin untuk approve/reject data ini" }, statusCode: 403);

            var item = await db.PeristiwaKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status != "menunggu") return Results.BadRequest(new { success = false, message = "Hanya data 'menunggu' yang dapat diproses" });
            if (req.Action != "disetujui" && req.Action != "ditolak") return Results.BadRequest(new { success = false, message = "Action tidak valid" });

            item.Status = req.Action; item.CatatanApproval = req.Catatan;
            item.ApprovedBy = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            item.ApprovedByRole = ctx.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            item.ApprovedAt = DateTime.UtcNow; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<PeristiwaKonflikDetailDto> { Success = true, Data = MapToDetail(item),
                Message = req.Action == "disetujui" ? "Data berhasil disetujui" : "Data berhasil ditolak" });
        }).WithName("ApprovePeristiwaKonflik").WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.PeristiwaKonfliks.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat dihapus" });
            db.PeristiwaKonfliks.Remove(item); await db.SaveChangesAsync();
            return Results.Ok(new { success = true, message = "Data berhasil dihapus" });
        }).WithName("DeletePeristiwaKonflik").WithOpenApi();
    }

    private static PeristiwaKonflikDetailDto MapToDetail(PeristiwaKonflik p) => new()
    {
        Id = p.Id.ToString(), Periode = p.Periode.ToString("yyyy-MM-dd"),
        NamaPeristiwa = p.NamaPeristiwa, SumberSebabKonflik = p.SumberSebabKonflik,
        LatarBelakangKejadian = p.LatarBelakangKejadian, DeskripsiAkibatPeristiwa = p.DeskripsiAkibatPeristiwa,
        KorbanKritis = p.KorbanKritis, KorbanLukaLuka = p.KorbanLukaLuka,
        KorbanMengungsi = p.KorbanMengungsi, KerugianMateril = p.KerugianMateril,
        UpayaPenanganan = p.UpayaPenanganan, UpayaPemulihan = p.UpayaPemulihan,
        Kabupaten = p.Kabupaten, Kecamatan = p.Kecamatan, Desa = p.Desa,
        AlamatDetail = p.AlamatDetail, TitikKoordinat = p.TitikKoordinat,
        BuktiFoto = p.BuktiFoto, SumberInformasi = p.SumberInformasi,
        Keterangan = p.Keterangan, SaranTindakLanjut = p.SaranTindakLanjut,
        TingkatRisiko = p.TingkatRisiko, Status = p.Status,
        CatatanApproval = p.CatatanApproval, ApprovedBy = p.ApprovedBy,
        ApprovedByRole = p.ApprovedByRole,
        ApprovedAt = p.ApprovedAt?.ToString("o"), CreatedBy = p.CreatedBy, CreatedAt = p.CreatedAt.ToString("o")
    };
}

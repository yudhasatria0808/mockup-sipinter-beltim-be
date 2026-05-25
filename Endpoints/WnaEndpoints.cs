using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class WnaEndpoints
{
    public static void MapWnaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wna")
            .WithTags("WNA").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, string? kewarganegaraan,
            string? statusTinggal, string? status, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.WargaNegaraAsings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(kewarganegaraan))
                query = query.Where(w => w.Kewarganegaraan == kewarganegaraan);
            if (!string.IsNullOrWhiteSpace(statusTinggal))
                query = query.Where(w => w.StatusTinggal == statusTinggal);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(w => w.Status == status);
            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var s = generalSearch.ToLower();
                query = query.Where(w =>
                    w.Kewarganegaraan.ToLower().Contains(s) ||
                    w.NoPaspor.ToLower().Contains(s) ||
                    w.Kabupaten.ToLower().Contains(s) ||
                    w.Kecamatan.ToLower().Contains(s) ||
                    w.Desa.ToLower().Contains(s));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);
            var items = await query.OrderByDescending(w => w.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .Select(w => new WnaListDto
                {
                    Id = w.Id.ToString(), Periode = w.Periode.ToString("yyyy-MM-dd"),
                    JenisKelamin = w.JenisKelamin, Kewarganegaraan = w.Kewarganegaraan,
                    NoPaspor = w.NoPaspor, JenisVisa = w.JenisVisa,
                    MasaBerlakuVisa = w.MasaBerlakuVisa.ToString("yyyy-MM-dd"),
                    Kabupaten = w.Kabupaten, Kecamatan = w.Kecamatan, Desa = w.Desa,
                    StatusTinggal = w.StatusTinggal, Status = w.Status,
                    CreatedBy = w.CreatedBy
                }).ToListAsync();

            return Results.Ok(new PaginatedResponse<WnaListDto>
            { Data = items, CurrentPage = page, PageSize = size, TotalPages = totalPages, TotalCount = totalCount });
        }).WithName("GetWna").WithOpenApi();

        // GET EWS (only disetujui)
        group.MapGet("/ews", async (string? kewarganegaraan, AppDbContext db) =>
        {
            var query = db.WargaNegaraAsings
                .Where(w => w.Status == "disetujui")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(kewarganegaraan))
                query = query.Where(w => w.Kewarganegaraan == kewarganegaraan);

            var allApproved = await query.ToListAsync();

            var items = allApproved.OrderByDescending(w => w.CreatedAt)
                .Select(w => MapToDetail(w)).ToList();

            var stats = new
            {
                total = allApproved.Count,
                byKewarganegaraan = allApproved.GroupBy(w => w.Kewarganegaraan)
                    .Select(g => new { kewarganegaraan = g.Key, count = g.Count() })
                    .OrderByDescending(g => g.count).Take(5).ToList(),
                byStatusTinggal = new
                {
                    aktif = allApproved.Count(w => w.StatusTinggal == "Aktif"),
                    keluar = allApproved.Count(w => w.StatusTinggal == "Keluar"),
                    habisIzin = allApproved.Count(w => w.StatusTinggal == "Habis Izin"),
                    lainnya = allApproved.Count(w => w.StatusTinggal == "Lainnya"),
                },
            };
            return Results.Ok(new ApiResponse<object> { Success = true, Data = new { stats, items } });
        }).WithName("GetWnaEWS").WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.WargaNegaraAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            return Results.Ok(new ApiResponse<WnaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("GetWnaById").WithOpenApi();

        // POST create
        group.MapPost("/", async (WnaCreateRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var userName = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = new WargaNegaraAsing
            {
                Periode = DateTime.Parse(req.Periode),
                JenisKelamin = req.JenisKelamin, Kewarganegaraan = req.Kewarganegaraan,
                NoPaspor = req.NoPaspor, JenisVisa = req.JenisVisa,
                MasaBerlakuVisa = DateTime.Parse(req.MasaBerlakuVisa),
                Pekerjaan = req.Pekerjaan, Sponsor = req.Sponsor,
                Kabupaten = req.Kabupaten, Kecamatan = req.Kecamatan, Desa = req.Desa,
                AlamatDetail = req.AlamatDetail, TitikKoordinat = req.TitikKoordinat,
                LamaTinggal = req.LamaTinggal, StatusTinggal = req.StatusTinggal,
                Keterangan = req.Keterangan, SumberInformasi = req.SumberInformasi,
                SaranTindakLanjut = req.SaranTindakLanjut, Status = req.Status,
                CreatedBy = userName, CreatedByUserId = userId != null ? Guid.Parse(userId) : null,
            };
            db.WargaNegaraAsings.Add(item); await db.SaveChangesAsync();
            return Results.Created($"/api/wna/{item.Id}",
                new ApiResponse<WnaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("CreateWna").WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, WnaUpdateRequest req, AppDbContext db) =>
        {
            var item = await db.WargaNegaraAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat diubah" });

            item.Periode = DateTime.Parse(req.Periode);
            item.JenisKelamin = req.JenisKelamin; item.Kewarganegaraan = req.Kewarganegaraan;
            item.NoPaspor = req.NoPaspor; item.JenisVisa = req.JenisVisa;
            item.MasaBerlakuVisa = DateTime.Parse(req.MasaBerlakuVisa);
            item.Pekerjaan = req.Pekerjaan; item.Sponsor = req.Sponsor;
            item.Kabupaten = req.Kabupaten; item.Kecamatan = req.Kecamatan; item.Desa = req.Desa;
            item.AlamatDetail = req.AlamatDetail; item.TitikKoordinat = req.TitikKoordinat;
            item.LamaTinggal = req.LamaTinggal; item.StatusTinggal = req.StatusTinggal;
            item.Keterangan = req.Keterangan; item.SumberInformasi = req.SumberInformasi;
            item.SaranTindakLanjut = req.SaranTindakLanjut;
            item.Status = req.Status; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<WnaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("UpdateWna").WithOpenApi();

        // POST approval
        group.MapPost("/{id}/approval", async (string id, KewaspadaanApprovalRequest req, HttpContext ctx, AppDbContext db) =>
        {
            // Check CanApprove permission
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Results.Unauthorized();
            var user = await db.Users.FindAsync(Guid.Parse(userId));
            if (user is null) return Results.Unauthorized();
            var menuId = Guid.Parse("b0000007-0000-0000-0000-000000000011"); // Form WNA
            var hasPerm = await db.RolePermissions.AnyAsync(rp => rp.RoleId == user.RoleId && rp.MenuId == menuId && rp.CanApprove);
            if (!hasPerm)
                return Results.Json(new { success = false, message = "Anda tidak memiliki izin untuk approve/reject data ini" }, statusCode: 403);

            var item = await db.WargaNegaraAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status != "menunggu") return Results.BadRequest(new { success = false, message = "Hanya data 'menunggu' yang dapat diproses" });
            if (req.Action != "disetujui" && req.Action != "ditolak") return Results.BadRequest(new { success = false, message = "Action tidak valid" });

            item.Status = req.Action; item.CatatanApproval = req.Catatan;
            item.ApprovedBy = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            item.ApprovedByRole = ctx.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            item.ApprovedAt = DateTime.UtcNow; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<WnaDetailDto> { Success = true, Data = MapToDetail(item),
                Message = req.Action == "disetujui" ? "Data berhasil disetujui" : "Data berhasil ditolak" });
        }).WithName("ApproveWna").WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.WargaNegaraAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat dihapus" });
            db.WargaNegaraAsings.Remove(item); await db.SaveChangesAsync();
            return Results.Ok(new { success = true, message = "Data berhasil dihapus" });
        }).WithName("DeleteWna").WithOpenApi();
    }

    private static WnaDetailDto MapToDetail(WargaNegaraAsing w) => new()
    {
        Id = w.Id.ToString(), Periode = w.Periode.ToString("yyyy-MM-dd"),
        JenisKelamin = w.JenisKelamin, Kewarganegaraan = w.Kewarganegaraan,
        NoPaspor = w.NoPaspor, JenisVisa = w.JenisVisa,
        MasaBerlakuVisa = w.MasaBerlakuVisa.ToString("yyyy-MM-dd"),
        Kabupaten = w.Kabupaten, Kecamatan = w.Kecamatan, Desa = w.Desa,
        StatusTinggal = w.StatusTinggal, Status = w.Status,
        CreatedBy = w.CreatedBy,
        Pekerjaan = w.Pekerjaan, Sponsor = w.Sponsor,
        AlamatDetail = w.AlamatDetail, TitikKoordinat = w.TitikKoordinat,
        LamaTinggal = w.LamaTinggal, Keterangan = w.Keterangan,
        SumberInformasi = w.SumberInformasi, SaranTindakLanjut = w.SaranTindakLanjut,
        CatatanApproval = w.CatatanApproval, ApprovedBy = w.ApprovedBy,
        ApprovedByRole = w.ApprovedByRole,
        ApprovedAt = w.ApprovedAt?.ToString("o"),
        CreatedAt = w.CreatedAt.ToString("o")
    };
}

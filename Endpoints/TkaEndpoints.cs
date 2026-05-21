using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class TkaEndpoints
{
    public static void MapTkaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tka")
            .WithTags("TKA").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, string? kewarganegaraan,
            string? status, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.TenagaKerjaAsings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(kewarganegaraan))
                query = query.Where(t => t.Kewarganegaraan == kewarganegaraan);
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);
            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var s = generalSearch.ToLower();
                query = query.Where(t =>
                    t.NamaTKA.ToLower().Contains(s) ||
                    t.NamaPerusahaan.ToLower().Contains(s) ||
                    t.Kabupaten.ToLower().Contains(s) ||
                    t.Kecamatan.ToLower().Contains(s) ||
                    t.Desa.ToLower().Contains(s));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);
            var items = await query.OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * size).Take(size)
                .Select(t => new TkaListDto
                {
                    Id = t.Id.ToString(), Periode = t.Periode.ToString("yyyy-MM-dd"),
                    NamaTKA = t.NamaTKA, JenisKelamin = t.JenisKelamin,
                    NamaPerusahaan = t.NamaPerusahaan, JabatanKeterampilan = t.JabatanKeterampilan,
                    Kewarganegaraan = t.Kewarganegaraan, JenisIzinTinggal = t.JenisIzinTinggal,
                    Kabupaten = t.Kabupaten, Kecamatan = t.Kecamatan, Desa = t.Desa,
                    Status = t.Status, CreatedBy = t.CreatedBy
                }).ToListAsync();

            return Results.Ok(new PaginatedResponse<TkaListDto>
            { Data = items, CurrentPage = page, PageSize = size, TotalPages = totalPages, TotalCount = totalCount });
        }).WithName("GetTka").WithOpenApi();

        // GET EWS (only disetujui)
        group.MapGet("/ews", async (string? kewarganegaraan, AppDbContext db) =>
        {
            var query = db.TenagaKerjaAsings
                .Where(t => t.Status == "disetujui")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(kewarganegaraan))
                query = query.Where(t => t.Kewarganegaraan == kewarganegaraan);

            var allApproved = await query.ToListAsync();

            var items = allApproved.OrderByDescending(t => t.CreatedAt)
                .Select(t => MapToDetail(t)).ToList();

            var stats = new
            {
                total = allApproved.Count,
                byKewarganegaraan = allApproved.GroupBy(t => t.Kewarganegaraan)
                    .Select(g => new { kewarganegaraan = g.Key, count = g.Count() })
                    .OrderByDescending(g => g.count).ToList(),
                byJenisIzin = new
                {
                    visa = allApproved.Count(t => t.JenisIzinTinggal == "Visa"),
                    kitas = allApproved.Count(t => t.JenisIzinTinggal == "KITAS"),
                    kitap = allApproved.Count(t => t.JenisIzinTinggal == "KITAP"),
                },
            };
            return Results.Ok(new ApiResponse<object> { Success = true, Data = new { stats, items } });
        }).WithName("GetTkaEWS").WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.TenagaKerjaAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            return Results.Ok(new ApiResponse<TkaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("GetTkaById").WithOpenApi();

        // POST create
        group.MapPost("/", async (TkaCreateRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var userName = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = new TenagaKerjaAsing
            {
                Periode = DateTime.Parse(req.Periode),
                NamaTKA = req.NamaTKA, JenisKelamin = req.JenisKelamin,
                NamaPerusahaan = req.NamaPerusahaan, JabatanKeterampilan = req.JabatanKeterampilan,
                NoTelepon = req.NoTelepon, Kewarganegaraan = req.Kewarganegaraan,
                NoPaspor = req.NoPaspor, NomorIMTA = req.NomorIMTA,
                TanggalMulaiIMTA = !string.IsNullOrEmpty(req.TanggalMulaiIMTA) ? DateTime.Parse(req.TanggalMulaiIMTA) : null,
                TanggalBerakhirIMTA = !string.IsNullOrEmpty(req.TanggalBerakhirIMTA) ? DateTime.Parse(req.TanggalBerakhirIMTA) : null,
                JenisIzinTinggal = req.JenisIzinTinggal,
                Kabupaten = req.Kabupaten, Kecamatan = req.Kecamatan, Desa = req.Desa,
                AlamatDetail = req.AlamatDetail, TitikKoordinat = req.TitikKoordinat,
                Keterangan = req.Keterangan, SumberInformasi = req.SumberInformasi,
                SaranTindakLanjut = req.SaranTindakLanjut, Status = req.Status,
                CreatedBy = userName, CreatedByUserId = userId != null ? Guid.Parse(userId) : null,
            };
            db.TenagaKerjaAsings.Add(item); await db.SaveChangesAsync();
            return Results.Created($"/api/tka/{item.Id}",
                new ApiResponse<TkaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("CreateTka").WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, TkaUpdateRequest req, AppDbContext db) =>
        {
            var item = await db.TenagaKerjaAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat diubah" });

            item.Periode = DateTime.Parse(req.Periode);
            item.NamaTKA = req.NamaTKA; item.JenisKelamin = req.JenisKelamin;
            item.NamaPerusahaan = req.NamaPerusahaan; item.JabatanKeterampilan = req.JabatanKeterampilan;
            item.NoTelepon = req.NoTelepon; item.Kewarganegaraan = req.Kewarganegaraan;
            item.NoPaspor = req.NoPaspor; item.NomorIMTA = req.NomorIMTA;
            item.TanggalMulaiIMTA = !string.IsNullOrEmpty(req.TanggalMulaiIMTA) ? DateTime.Parse(req.TanggalMulaiIMTA) : null;
            item.TanggalBerakhirIMTA = !string.IsNullOrEmpty(req.TanggalBerakhirIMTA) ? DateTime.Parse(req.TanggalBerakhirIMTA) : null;
            item.JenisIzinTinggal = req.JenisIzinTinggal;
            item.Kabupaten = req.Kabupaten; item.Kecamatan = req.Kecamatan; item.Desa = req.Desa;
            item.AlamatDetail = req.AlamatDetail; item.TitikKoordinat = req.TitikKoordinat;
            item.Keterangan = req.Keterangan; item.SumberInformasi = req.SumberInformasi;
            item.SaranTindakLanjut = req.SaranTindakLanjut;
            item.Status = req.Status; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<TkaDetailDto> { Success = true, Data = MapToDetail(item) });
        }).WithName("UpdateTka").WithOpenApi();

        // POST approval
        group.MapPost("/{id}/approval", async (string id, KewaspadaanApprovalRequest req, HttpContext ctx, AppDbContext db) =>
        {
            var item = await db.TenagaKerjaAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status != "menunggu") return Results.BadRequest(new { success = false, message = "Hanya data 'menunggu' yang dapat diproses" });
            if (req.Action != "disetujui" && req.Action != "ditolak") return Results.BadRequest(new { success = false, message = "Action tidak valid" });

            item.Status = req.Action; item.CatatanApproval = req.Catatan;
            item.ApprovedBy = ctx.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            item.ApprovedAt = DateTime.UtcNow; item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new ApiResponse<TkaDetailDto> { Success = true, Data = MapToDetail(item),
                Message = req.Action == "disetujui" ? "Data berhasil disetujui" : "Data berhasil ditolak" });
        }).WithName("ApproveTka").WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.TenagaKerjaAsings.FindAsync(Guid.Parse(id));
            if (item is null) return Results.NotFound(new { success = false, message = "Data tidak ditemukan" });
            if (item.Status == "disetujui") return Results.BadRequest(new { success = false, message = "Data yang sudah disetujui tidak dapat dihapus" });
            db.TenagaKerjaAsings.Remove(item); await db.SaveChangesAsync();
            return Results.Ok(new { success = true, message = "Data berhasil dihapus" });
        }).WithName("DeleteTka").WithOpenApi();
    }

    private static TkaDetailDto MapToDetail(TenagaKerjaAsing t) => new()
    {
        Id = t.Id.ToString(), Periode = t.Periode.ToString("yyyy-MM-dd"),
        NamaTKA = t.NamaTKA, JenisKelamin = t.JenisKelamin,
        NamaPerusahaan = t.NamaPerusahaan, JabatanKeterampilan = t.JabatanKeterampilan,
        Kewarganegaraan = t.Kewarganegaraan, JenisIzinTinggal = t.JenisIzinTinggal,
        Kabupaten = t.Kabupaten, Kecamatan = t.Kecamatan, Desa = t.Desa,
        Status = t.Status, CreatedBy = t.CreatedBy,
        NoPaspor = t.NoPaspor, NoTelepon = t.NoTelepon,
        NomorIMTA = t.NomorIMTA,
        TanggalMulaiIMTA = t.TanggalMulaiIMTA?.ToString("yyyy-MM-dd"),
        TanggalBerakhirIMTA = t.TanggalBerakhirIMTA?.ToString("yyyy-MM-dd"),
        AlamatDetail = t.AlamatDetail, TitikKoordinat = t.TitikKoordinat,
        Keterangan = t.Keterangan, SumberInformasi = t.SumberInformasi,
        SaranTindakLanjut = t.SaranTindakLanjut,
        CatatanApproval = t.CatatanApproval, ApprovedBy = t.ApprovedBy,
        ApprovedAt = t.ApprovedAt?.ToString("o"),
        CreatedAt = t.CreatedAt.ToString("o")
    };
}

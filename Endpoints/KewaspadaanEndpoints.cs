using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class KewaspadaanEndpoints
{
    public static void MapKewaspadaanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/kewaspadaan")
            .WithTags("Kewaspadaan Dini").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (
            string? generalSearch, string? aspek, string? status,
            int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.KewaspadaanDinis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(aspek))
                query = query.Where(k => k.Aspek == aspek);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(k => k.Status == status);

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(k =>
                    k.Aspek.ToLower().Contains(search) ||
                    k.Kabupaten.ToLower().Contains(search) ||
                    k.Kecamatan.ToLower().Contains(search) ||
                    k.Desa.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderByDescending(k => k.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(k => new KewaspadaanListDto
                {
                    Id = k.Id.ToString(),
                    Periode = k.Periode.ToString("yyyy-MM-dd"),
                    Aspek = k.Aspek,
                    Kabupaten = k.Kabupaten,
                    Kecamatan = k.Kecamatan,
                    Desa = k.Desa,
                    TingkatRisiko = k.TingkatRisiko,
                    Status = k.Status,
                    CreatedBy = k.CreatedBy,
                    CreatedAt = k.CreatedAt.ToString("o")
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<KewaspadaanListDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetKewaspadaan")
        .WithOpenApi();

        // GET EWS (only disetujui)
        group.MapGet("/ews", async (string? aspek, string? tingkatRisiko,
            AppDbContext db) =>
        {
            var query = db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(aspek))
                query = query.Where(k => k.Aspek == aspek);

            if (!string.IsNullOrWhiteSpace(tingkatRisiko))
                query = query.Where(k => k.TingkatRisiko == tingkatRisiko);

            var items = await query
                .OrderByDescending(k => k.CreatedAt)
                .Select(k => MapToDetail(k))
                .ToListAsync();

            // Stats
            var allApproved = await db.KewaspadaanDinis
                .Where(k => k.Status == "disetujui")
                .ToListAsync();

            var stats = new
            {
                total = allApproved.Count,
                sangatTinggi = allApproved.Count(k => k.TingkatRisiko == "Sangat Tinggi"),
                tinggi = allApproved.Count(k => k.TingkatRisiko == "Tinggi"),
                sedang = allApproved.Count(k => k.TingkatRisiko == "Sedang"),
                rendah = allApproved.Count(k => k.TingkatRisiko == "Rendah"),
            };

            return Results.Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { stats, items }
            });
        })
        .WithName("GetKewaspadaanEWS")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.KewaspadaanDinis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Data kewaspadaan tidak ditemukan" });

            return Results.Ok(new ApiResponse<KewaspadaanDetailDto>
            {
                Success = true,
                Data = MapToDetail(item)
            });
        })
        .WithName("GetKewaspadaanById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (KewaspadaanCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var item = new KewaspadaanDini
            {
                Periode = DateTime.Parse(request.Periode),
                Aspek = request.Aspek,
                Kabupaten = request.Kabupaten,
                Kecamatan = request.Kecamatan,
                Desa = request.Desa,
                AlamatDetail = request.AlamatDetail,
                TitikKoordinat = request.TitikKoordinat,
                SumberInformasi = request.SumberInformasi,
                KemungkinanAncamanLevel = request.KemungkinanLevel,
                KemungkinanAncamanDeskripsi = request.KemungkinanDeskripsi,
                Hambatan = request.Hambatan,
                Tantangan = request.Tantangan,
                Gangguan = request.Gangguan,
                PrediksiDampakLevel = request.DampakLevel,
                PrediksiDampakDeskripsi = request.DampakDeskripsi,
                Rekomendasi = request.Rekomendasi,
                TingkatRisiko = request.TingkatRisiko,
                Status = request.Status,
                CreatedByUserId = userId != null ? Guid.Parse(userId) : null,
                CreatedBy = userName,
            };

            db.KewaspadaanDinis.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/kewaspadaan/{item.Id}",
                new ApiResponse<KewaspadaanDetailDto>
            {
                Success = true,
                Data = MapToDetail(item)
            });
        })
        .WithName("CreateKewaspadaan")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id,
            KewaspadaanUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.KewaspadaanDinis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Data kewaspadaan tidak ditemukan" });

            if (item.Status == "disetujui")
                return Results.BadRequest(new { success = false,
                    message = "Data yang sudah disetujui tidak dapat diubah" });

            item.Periode = DateTime.Parse(request.Periode);
            item.Aspek = request.Aspek;
            item.Kabupaten = request.Kabupaten;
            item.Kecamatan = request.Kecamatan;
            item.Desa = request.Desa;
            item.AlamatDetail = request.AlamatDetail;
            item.TitikKoordinat = request.TitikKoordinat;
            item.SumberInformasi = request.SumberInformasi;
            item.KemungkinanAncamanLevel = request.KemungkinanLevel;
            item.KemungkinanAncamanDeskripsi = request.KemungkinanDeskripsi;
            item.Hambatan = request.Hambatan;
            item.Tantangan = request.Tantangan;
            item.Gangguan = request.Gangguan;
            item.PrediksiDampakLevel = request.DampakLevel;
            item.PrediksiDampakDeskripsi = request.DampakDeskripsi;
            item.Rekomendasi = request.Rekomendasi;
            item.TingkatRisiko = request.TingkatRisiko;
            item.Status = request.Status;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<KewaspadaanDetailDto>
            {
                Success = true,
                Data = MapToDetail(item)
            });
        })
        .WithName("UpdateKewaspadaan")
        .WithOpenApi();

        // POST approval
        group.MapPost("/{id}/approval", async (string id,
            KewaspadaanApprovalRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var item = await db.KewaspadaanDinis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Data kewaspadaan tidak ditemukan" });

            if (item.Status != "menunggu")
                return Results.BadRequest(new { success = false,
                    message = "Hanya data dengan status 'menunggu' yang dapat diproses" });

            if (request.Action != "disetujui" && request.Action != "ditolak")
                return Results.BadRequest(new { success = false,
                    message = "Action harus 'disetujui' atau 'ditolak'" });

            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            item.Status = request.Action;
            item.CatatanApproval = request.Catatan;
            item.ApprovedBy = userName;
            item.ApprovedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<KewaspadaanDetailDto>
            {
                Success = true,
                Data = MapToDetail(item),
                Message = request.Action == "disetujui"
                    ? "Data berhasil disetujui"
                    : "Data berhasil ditolak"
            });
        })
        .WithName("ApproveKewaspadaan")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.KewaspadaanDinis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Data kewaspadaan tidak ditemukan" });

            if (item.Status == "disetujui")
                return Results.BadRequest(new { success = false,
                    message = "Data yang sudah disetujui tidak dapat dihapus" });

            db.KewaspadaanDinis.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Data kewaspadaan berhasil dihapus" });
        })
        .WithName("DeleteKewaspadaan")
        .WithOpenApi();
    }

    private static KewaspadaanDetailDto MapToDetail(KewaspadaanDini k)
    {
        return new KewaspadaanDetailDto
        {
            Id = k.Id.ToString(),
            Periode = k.Periode.ToString("yyyy-MM-dd"),
            Aspek = k.Aspek,
            Kabupaten = k.Kabupaten,
            Kecamatan = k.Kecamatan,
            Desa = k.Desa,
            AlamatDetail = k.AlamatDetail,
            TitikKoordinat = k.TitikKoordinat,
            BuktiFoto = k.BuktiFoto,
            SumberInformasi = k.SumberInformasi,
            KemungkinanAncaman = new KemungkinanAncamanDto
            {
                Level = k.KemungkinanAncamanLevel,
                Deskripsi = k.KemungkinanAncamanDeskripsi
            },
            Hambatan = k.Hambatan,
            Tantangan = k.Tantangan,
            Gangguan = k.Gangguan,
            PrediksiDampak = new PrediksiDampakDto
            {
                Level = k.PrediksiDampakLevel,
                Deskripsi = k.PrediksiDampakDeskripsi
            },
            Rekomendasi = k.Rekomendasi,
            TingkatRisiko = k.TingkatRisiko,
            Status = k.Status,
            CatatanApproval = k.CatatanApproval,
            ApprovedBy = k.ApprovedBy,
            ApprovedAt = k.ApprovedAt?.ToString("o"),
            CreatedBy = k.CreatedBy,
            CreatedAt = k.CreatedAt.ToString("o")
        };
    }
}

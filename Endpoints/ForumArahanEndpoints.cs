using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class ForumArahanEndpoints
{
    public static void MapForumArahanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/forum/arahan")
            .WithTags("Forum - Arahan & Disposisi").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (
            string? generalSearch, string? prioritas, string? instansiTujuan,
            int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.ForumArahans
                .Include(a => a.ForumTopik)
                .Include(a => a.TindakLanjuts)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(prioritas))
                query = query.Where(a => a.Prioritas == prioritas);

            if (!string.IsNullOrWhiteSpace(instansiTujuan))
            {
                var inst = instansiTujuan.ToLower();
                query = query.Where(a => a.InstansiTujuan.ToLower().Contains(inst));
            }

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(a =>
                    a.Judul.ToLower().Contains(search) ||
                    a.Isi.ToLower().Contains(search) ||
                    a.InstansiTujuan.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a => new ForumArahanListDto
                {
                    Id = a.Id.ToString(),
                    Judul = a.Judul,
                    Prioritas = a.Prioritas,
                    InstansiTujuan = a.InstansiTujuan,
                    ForumTopikId = a.ForumTopikId != null ? a.ForumTopikId.ToString() : null,
                    ForumTopikJudul = a.ForumTopik != null ? a.ForumTopik.Judul : null,
                    JumlahTindakLanjut = a.TindakLanjuts.Count,
                    TindakLanjutSelesai = a.TindakLanjuts.Count(tl => tl.Status == "selesai"),
                    CreatedByName = a.CreatedByName,
                    CreatedByRole = a.CreatedByRole,
                    CreatedAt = a.CreatedAt.ToString("o")
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<ForumArahanListDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetForumArahan")
        .WithOpenApi();

        // GET by id (with tindak lanjut)
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumArahans
                .Include(a => a.ForumTopik)
                .Include(a => a.TindakLanjuts.OrderByDescending(tl => tl.CreatedAt))
                .FirstOrDefaultAsync(a => a.Id == Guid.Parse(id));

            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Arahan tidak ditemukan" });

            var dto = new ForumArahanDetailDto
            {
                Id = item.Id.ToString(),
                Judul = item.Judul,
                Isi = item.Isi,
                Prioritas = item.Prioritas,
                InstansiTujuan = item.InstansiTujuan,
                ForumTopikId = item.ForumTopikId?.ToString(),
                ForumTopikJudul = item.ForumTopik?.Judul,
                CreatedByName = item.CreatedByName,
                CreatedByRole = item.CreatedByRole,
                CreatedAt = item.CreatedAt.ToString("o"),
                TindakLanjuts = item.TindakLanjuts.Select(tl => new ForumTindakLanjutDto
                {
                    Id = tl.Id.ToString(),
                    Isi = tl.Isi,
                    Status = tl.Status,
                    TanggalSelesai = tl.TanggalSelesai?.ToString("yyyy-MM-dd"),
                    CreatedByName = tl.CreatedByName,
                    CreatedByRole = tl.CreatedByRole,
                    CreatedAt = tl.CreatedAt.ToString("o")
                }).ToList()
            };

            return Results.Ok(new ApiResponse<ForumArahanDetailDto>
            {
                Success = true,
                Data = dto
            });
        })
        .WithName("GetForumArahanById")
        .WithOpenApi();

        // POST create arahan
        group.MapPost("/", async (ForumArahanCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var item = new ForumArahan
            {
                Judul = request.Judul,
                Isi = request.Isi,
                Prioritas = request.Prioritas,
                InstansiTujuan = request.InstansiTujuan,
                ForumTopikId = !string.IsNullOrEmpty(request.ForumTopikId)
                    ? Guid.Parse(request.ForumTopikId) : null,
                CreatedByUserId = Guid.Parse(userId!),
                CreatedByName = userName,
                CreatedByRole = userRole
            };

            db.ForumArahans.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/forum/arahan/{item.Id}",
                new ApiResponse<ForumArahanDetailDto>
                {
                    Success = true,
                    Data = new ForumArahanDetailDto
                    {
                        Id = item.Id.ToString(),
                        Judul = item.Judul,
                        Isi = item.Isi,
                        Prioritas = item.Prioritas,
                        InstansiTujuan = item.InstansiTujuan,
                        ForumTopikId = item.ForumTopikId?.ToString(),
                        CreatedByName = item.CreatedByName,
                        CreatedByRole = item.CreatedByRole,
                        CreatedAt = item.CreatedAt.ToString("o"),
                        TindakLanjuts = new()
                    },
                    Message = "Arahan berhasil dibuat"
                });
        })
        .WithName("CreateForumArahan")
        .WithOpenApi();

        // PUT update arahan
        group.MapPut("/{id}", async (string id, ForumArahanUpdateRequest request,
            AppDbContext db) =>
        {
            var item = await db.ForumArahans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Arahan tidak ditemukan" });

            item.Judul = request.Judul;
            item.Isi = request.Isi;
            item.Prioritas = request.Prioritas;
            item.InstansiTujuan = request.InstansiTujuan;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { id = item.Id.ToString() },
                Message = "Arahan berhasil diperbarui"
            });
        })
        .WithName("UpdateForumArahan")
        .WithOpenApi();

        // DELETE arahan
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumArahans
                .Include(a => a.TindakLanjuts)
                .FirstOrDefaultAsync(a => a.Id == Guid.Parse(id));

            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Arahan tidak ditemukan" });

            db.ForumTindakLanjuts.RemoveRange(item.TindakLanjuts);
            db.ForumArahans.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Arahan berhasil dihapus" });
        })
        .WithName("DeleteForumArahan")
        .WithOpenApi();

        // POST add tindak lanjut to arahan
        group.MapPost("/{id}/tindak-lanjut", async (string id,
            ForumTindakLanjutCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var arahan = await db.ForumArahans.FindAsync(Guid.Parse(id));
            if (arahan is null)
                return Results.NotFound(new { success = false,
                    message = "Arahan tidak ditemukan" });

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var tindakLanjut = new ForumTindakLanjut
            {
                ForumArahanId = arahan.Id,
                Isi = request.Isi,
                CreatedByUserId = Guid.Parse(userId!),
                CreatedByName = userName,
                CreatedByRole = userRole
            };

            db.ForumTindakLanjuts.Add(tindakLanjut);
            await db.SaveChangesAsync();

            return Results.Created($"/api/forum/arahan/{id}/tindak-lanjut/{tindakLanjut.Id}",
                new ApiResponse<ForumTindakLanjutDto>
                {
                    Success = true,
                    Data = new ForumTindakLanjutDto
                    {
                        Id = tindakLanjut.Id.ToString(),
                        Isi = tindakLanjut.Isi,
                        Status = tindakLanjut.Status,
                        CreatedByName = tindakLanjut.CreatedByName,
                        CreatedByRole = tindakLanjut.CreatedByRole,
                        CreatedAt = tindakLanjut.CreatedAt.ToString("o")
                    },
                    Message = "Tindak lanjut berhasil ditambahkan"
                });
        })
        .WithName("CreateForumTindakLanjut")
        .WithOpenApi();

        // PUT update status tindak lanjut
        group.MapPut("/{arahanId}/tindak-lanjut/{tindakLanjutId}/status", async (
            string arahanId, string tindakLanjutId,
            ForumTindakLanjutUpdateStatusRequest request, AppDbContext db) =>
        {
            var tl = await db.ForumTindakLanjuts
                .FirstOrDefaultAsync(t => t.Id == Guid.Parse(tindakLanjutId)
                    && t.ForumArahanId == Guid.Parse(arahanId));

            if (tl is null)
                return Results.NotFound(new { success = false,
                    message = "Tindak lanjut tidak ditemukan" });

            var validStatuses = new[] { "belum", "sedang", "selesai" };
            if (!validStatuses.Contains(request.Status))
                return Results.BadRequest(new { success = false,
                    message = "Status harus 'belum', 'sedang', atau 'selesai'" });

            tl.Status = request.Status;
            tl.UpdatedAt = DateTime.UtcNow;
            if (request.Status == "selesai")
                tl.TanggalSelesai = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<ForumTindakLanjutDto>
            {
                Success = true,
                Data = new ForumTindakLanjutDto
                {
                    Id = tl.Id.ToString(),
                    Isi = tl.Isi,
                    Status = tl.Status,
                    TanggalSelesai = tl.TanggalSelesai?.ToString("yyyy-MM-dd"),
                    CreatedByName = tl.CreatedByName,
                    CreatedByRole = tl.CreatedByRole,
                    CreatedAt = tl.CreatedAt.ToString("o")
                },
                Message = "Status tindak lanjut berhasil diperbarui"
            });
        })
        .WithName("UpdateForumTindakLanjutStatus")
        .WithOpenApi();
    }
}

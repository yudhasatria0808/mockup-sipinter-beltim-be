using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class ForumTopikEndpoints
{
    public static void MapForumTopikEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/forum/topik")
            .WithTags("Forum - Topik Diskusi").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (
            string? generalSearch, string? kategori, string? prioritas, string? status,
            int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.ForumTopiks
                .Include(t => t.Komentars)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(kategori))
                query = query.Where(t => t.Kategori == kategori);

            if (!string.IsNullOrWhiteSpace(prioritas))
                query = query.Where(t => t.Prioritas == prioritas);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(t =>
                    t.Judul.ToLower().Contains(search) ||
                    t.Isi.ToLower().Contains(search) ||
                    t.CreatedByName.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(t => new ForumTopikListDto
                {
                    Id = t.Id.ToString(),
                    Judul = t.Judul,
                    Kategori = t.Kategori,
                    Prioritas = t.Prioritas,
                    Status = t.Status,
                    JumlahKomentar = t.Komentars.Count,
                    CreatedByName = t.CreatedByName,
                    CreatedByRole = t.CreatedByRole,
                    CreatedAt = t.CreatedAt.ToString("o"),
                    LastActivityAt = t.Komentars.Any()
                        ? t.Komentars.Max(k => k.CreatedAt).ToString("o")
                        : null
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<ForumTopikListDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetForumTopik")
        .WithOpenApi();

        // GET by id (with comments)
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumTopiks
                .Include(t => t.Komentars.OrderBy(k => k.CreatedAt))
                .FirstOrDefaultAsync(t => t.Id == Guid.Parse(id));

            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Topik diskusi tidak ditemukan" });

            var dto = new ForumTopikDetailDto
            {
                Id = item.Id.ToString(),
                Judul = item.Judul,
                Isi = item.Isi,
                Kategori = item.Kategori,
                Prioritas = item.Prioritas,
                Status = item.Status,
                KewaspadaanDiniId = item.KewaspadaanDiniId?.ToString(),
                PeristiwaKonflikId = item.PeristiwaKonflikId?.ToString(),
                CreatedByName = item.CreatedByName,
                CreatedByRole = item.CreatedByRole,
                CreatedAt = item.CreatedAt.ToString("o"),
                Komentars = item.Komentars.Select(k => new ForumKomentarDto
                {
                    Id = k.Id.ToString(),
                    Isi = k.Isi,
                    CreatedByName = k.CreatedByName,
                    CreatedByRole = k.CreatedByRole,
                    CreatedAt = k.CreatedAt.ToString("o")
                }).ToList()
            };

            return Results.Ok(new ApiResponse<ForumTopikDetailDto>
            {
                Success = true,
                Data = dto
            });
        })
        .WithName("GetForumTopikById")
        .WithOpenApi();

        // POST create topik
        group.MapPost("/", async (ForumTopikCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var item = new ForumTopik
            {
                Judul = request.Judul,
                Isi = request.Isi,
                Kategori = request.Kategori,
                Prioritas = request.Prioritas,
                KewaspadaanDiniId = !string.IsNullOrEmpty(request.KewaspadaanDiniId)
                    ? Guid.Parse(request.KewaspadaanDiniId) : null,
                PeristiwaKonflikId = !string.IsNullOrEmpty(request.PeristiwaKonflikId)
                    ? Guid.Parse(request.PeristiwaKonflikId) : null,
                CreatedByUserId = Guid.Parse(userId!),
                CreatedByName = userName,
                CreatedByRole = userRole
            };

            db.ForumTopiks.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/forum/topik/{item.Id}",
                new ApiResponse<ForumTopikDetailDto>
                {
                    Success = true,
                    Data = new ForumTopikDetailDto
                    {
                        Id = item.Id.ToString(),
                        Judul = item.Judul,
                        Isi = item.Isi,
                        Kategori = item.Kategori,
                        Prioritas = item.Prioritas,
                        Status = item.Status,
                        KewaspadaanDiniId = item.KewaspadaanDiniId?.ToString(),
                        PeristiwaKonflikId = item.PeristiwaKonflikId?.ToString(),
                        CreatedByName = item.CreatedByName,
                        CreatedByRole = item.CreatedByRole,
                        CreatedAt = item.CreatedAt.ToString("o"),
                        Komentars = new()
                    }
                });
        })
        .WithName("CreateForumTopik")
        .WithOpenApi();

        // PUT update topik
        group.MapPut("/{id}", async (string id, ForumTopikUpdateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var item = await db.ForumTopiks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Topik diskusi tidak ditemukan" });

            item.Judul = request.Judul;
            item.Isi = request.Isi;
            item.Kategori = request.Kategori;
            item.Prioritas = request.Prioritas;
            item.Status = request.Status;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { id = item.Id.ToString() },
                Message = "Topik diskusi berhasil diperbarui"
            });
        })
        .WithName("UpdateForumTopik")
        .WithOpenApi();

        // DELETE topik
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumTopiks
                .Include(t => t.Komentars)
                .FirstOrDefaultAsync(t => t.Id == Guid.Parse(id));

            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Topik diskusi tidak ditemukan" });

            db.ForumKomentars.RemoveRange(item.Komentars);
            db.ForumTopiks.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Topik diskusi berhasil dihapus" });
        })
        .WithName("DeleteForumTopik")
        .WithOpenApi();

        // POST add komentar to topik
        group.MapPost("/{id}/komentar", async (string id,
            ForumKomentarCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var topik = await db.ForumTopiks.FindAsync(Guid.Parse(id));
            if (topik is null)
                return Results.NotFound(new { success = false,
                    message = "Topik diskusi tidak ditemukan" });

            if (topik.Status != "aktif")
                return Results.BadRequest(new { success = false,
                    message = "Topik sudah ditutup, tidak bisa menambah komentar" });

            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var komentar = new ForumKomentar
            {
                ForumTopikId = topik.Id,
                Isi = request.Isi,
                CreatedByUserId = Guid.Parse(userId!),
                CreatedByName = userName,
                CreatedByRole = userRole
            };

            db.ForumKomentars.Add(komentar);
            topik.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Created($"/api/forum/topik/{id}/komentar/{komentar.Id}",
                new ApiResponse<ForumKomentarDto>
                {
                    Success = true,
                    Data = new ForumKomentarDto
                    {
                        Id = komentar.Id.ToString(),
                        Isi = komentar.Isi,
                        CreatedByName = komentar.CreatedByName,
                        CreatedByRole = komentar.CreatedByRole,
                        CreatedAt = komentar.CreatedAt.ToString("o")
                    }
                });
        })
        .WithName("CreateForumKomentar")
        .WithOpenApi();

        // DELETE komentar
        group.MapDelete("/{topikId}/komentar/{komentarId}", async (
            string topikId, string komentarId, AppDbContext db) =>
        {
            var komentar = await db.ForumKomentars
                .FirstOrDefaultAsync(k => k.Id == Guid.Parse(komentarId)
                    && k.ForumTopikId == Guid.Parse(topikId));

            if (komentar is null)
                return Results.NotFound(new { success = false,
                    message = "Komentar tidak ditemukan" });

            db.ForumKomentars.Remove(komentar);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Komentar berhasil dihapus" });
        })
        .WithName("DeleteForumKomentar")
        .WithOpenApi();
    }
}

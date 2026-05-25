using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class ForumPengumumanEndpoints
{
    public static void MapForumPengumumanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/forum/pengumuman")
            .WithTags("Forum - Pengumuman").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (
            string? generalSearch, string? prioritas, bool? isActive,
            int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.ForumPengumumans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(prioritas))
                query = query.Where(p => p.Prioritas == prioritas);

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(p =>
                    p.Judul.ToLower().Contains(search) ||
                    p.Isi.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(p => new ForumPengumumanListDto
                {
                    Id = p.Id.ToString(),
                    Judul = p.Judul,
                    Prioritas = p.Prioritas,
                    IsActive = p.IsActive,
                    CreatedByName = p.CreatedByName,
                    CreatedByRole = p.CreatedByRole,
                    CreatedAt = p.CreatedAt.ToString("o")
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<ForumPengumumanListDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetForumPengumuman")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumPengumumans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Pengumuman tidak ditemukan" });

            return Results.Ok(new ApiResponse<ForumPengumumanDetailDto>
            {
                Success = true,
                Data = new ForumPengumumanDetailDto
                {
                    Id = item.Id.ToString(),
                    Judul = item.Judul,
                    Isi = item.Isi,
                    Prioritas = item.Prioritas,
                    IsActive = item.IsActive,
                    CreatedByName = item.CreatedByName,
                    CreatedByRole = item.CreatedByRole,
                    CreatedAt = item.CreatedAt.ToString("o")
                }
            });
        })
        .WithName("GetForumPengumumanById")
        .WithOpenApi();

        // POST create pengumuman
        group.MapPost("/", async (ForumPengumumanCreateRequest request,
            HttpContext httpContext, AppDbContext db) =>
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var userRole = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            var item = new ForumPengumuman
            {
                Judul = request.Judul,
                Isi = request.Isi,
                Prioritas = request.Prioritas,
                CreatedByUserId = Guid.Parse(userId!),
                CreatedByName = userName,
                CreatedByRole = userRole
            };

            db.ForumPengumumans.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/forum/pengumuman/{item.Id}",
                new ApiResponse<ForumPengumumanDetailDto>
                {
                    Success = true,
                    Data = new ForumPengumumanDetailDto
                    {
                        Id = item.Id.ToString(),
                        Judul = item.Judul,
                        Isi = item.Isi,
                        Prioritas = item.Prioritas,
                        IsActive = item.IsActive,
                        CreatedByName = item.CreatedByName,
                        CreatedByRole = item.CreatedByRole,
                        CreatedAt = item.CreatedAt.ToString("o")
                    },
                    Message = "Pengumuman berhasil dibuat"
                });
        })
        .WithName("CreateForumPengumuman")
        .WithOpenApi();

        // PUT update pengumuman
        group.MapPut("/{id}", async (string id, ForumPengumumanUpdateRequest request,
            AppDbContext db) =>
        {
            var item = await db.ForumPengumumans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Pengumuman tidak ditemukan" });

            item.Judul = request.Judul;
            item.Isi = request.Isi;
            item.Prioritas = request.Prioritas;
            item.IsActive = request.IsActive;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<ForumPengumumanDetailDto>
            {
                Success = true,
                Data = new ForumPengumumanDetailDto
                {
                    Id = item.Id.ToString(),
                    Judul = item.Judul,
                    Isi = item.Isi,
                    Prioritas = item.Prioritas,
                    IsActive = item.IsActive,
                    CreatedByName = item.CreatedByName,
                    CreatedByRole = item.CreatedByRole,
                    CreatedAt = item.CreatedAt.ToString("o")
                },
                Message = "Pengumuman berhasil diperbarui"
            });
        })
        .WithName("UpdateForumPengumuman")
        .WithOpenApi();

        // DELETE pengumuman
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.ForumPengumumans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Pengumuman tidak ditemukan" });

            db.ForumPengumumans.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Pengumuman berhasil dihapus" });
        })
        .WithName("DeleteForumPengumuman")
        .WithOpenApi();
    }
}

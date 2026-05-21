using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class LevelKemungkinanEndpoints
{
    public static void MapLevelKemungkinanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/level-kemungkinan")
            .WithTags("Level Kemungkinan").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber,
            int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.LevelKemungkinans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(l =>
                    l.Nama.ToLower().Contains(search) ||
                    (l.Deskripsi != null &&
                     l.Deskripsi.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(l => l.Skor)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(l => new LevelKemungkinanDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Skor = l.Skor,
                    Deskripsi = l.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<LevelKemungkinanDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetLevelKemungkinan")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.LevelKemungkinans
                .OrderBy(l => l.Skor)
                .Select(l => new LevelKemungkinanDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Skor = l.Skor,
                    Deskripsi = l.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<LevelKemungkinanDto>>
                { Success = true, Data = items });
        })
        .WithName("GetAllLevelKemungkinan")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelKemungkinans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level kemungkinan tidak ditemukan" });

            return Results.Ok(new ApiResponse<LevelKemungkinanDto>
            {
                Success = true,
                Data = new LevelKemungkinanDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("GetLevelKemungkinanById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (LevelKemungkinanCreateRequest request,
            AppDbContext db) =>
        {
            if (await db.LevelKemungkinans.AnyAsync(l => l.Skor == request.Skor))
                return Results.BadRequest(new { success = false,
                    message = "Skor sudah digunakan" });

            var item = new LevelKemungkinan
            {
                Nama = request.Nama,
                Skor = request.Skor,
                Deskripsi = request.Deskripsi
            };

            db.LevelKemungkinans.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/level-kemungkinan/{item.Id}",
                new ApiResponse<LevelKemungkinanDto>
            {
                Success = true,
                Data = new LevelKemungkinanDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("CreateLevelKemungkinan")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id,
            LevelKemungkinanUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.LevelKemungkinans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level kemungkinan tidak ditemukan" });

            if (await db.LevelKemungkinans.AnyAsync(l =>
                l.Skor == request.Skor && l.Id != item.Id))
                return Results.BadRequest(new { success = false,
                    message = "Skor sudah digunakan" });

            item.Nama = request.Nama;
            item.Skor = request.Skor;
            item.Deskripsi = request.Deskripsi;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<LevelKemungkinanDto>
            {
                Success = true,
                Data = new LevelKemungkinanDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("UpdateLevelKemungkinan")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelKemungkinans.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level kemungkinan tidak ditemukan" });

            if (await db.MatriksRisikos.AnyAsync(m =>
                m.KemungkinanId == item.Id))
                return Results.BadRequest(new { success = false,
                    message = "Level kemungkinan masih digunakan di matriks" });

            db.LevelKemungkinans.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Level kemungkinan berhasil dihapus" });
        })
        .WithName("DeleteLevelKemungkinan")
        .WithOpenApi();
    }
}

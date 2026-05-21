using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class LevelDampakEndpoints
{
    public static void MapLevelDampakEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/level-dampak")
            .WithTags("Level Dampak").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber,
            int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.LevelDampaks.AsQueryable();

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
                .Select(l => new LevelDampakDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Skor = l.Skor,
                    Deskripsi = l.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<LevelDampakDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetLevelDampak")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.LevelDampaks
                .OrderBy(l => l.Skor)
                .Select(l => new LevelDampakDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Skor = l.Skor,
                    Deskripsi = l.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<LevelDampakDto>>
                { Success = true, Data = items });
        })
        .WithName("GetAllLevelDampak")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelDampaks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level dampak tidak ditemukan" });

            return Results.Ok(new ApiResponse<LevelDampakDto>
            {
                Success = true,
                Data = new LevelDampakDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("GetLevelDampakById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (LevelDampakCreateRequest request,
            AppDbContext db) =>
        {
            if (await db.LevelDampaks.AnyAsync(l => l.Skor == request.Skor))
                return Results.BadRequest(new { success = false,
                    message = "Skor sudah digunakan" });

            var item = new LevelDampak
            {
                Nama = request.Nama,
                Skor = request.Skor,
                Deskripsi = request.Deskripsi
            };

            db.LevelDampaks.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/level-dampak/{item.Id}",
                new ApiResponse<LevelDampakDto>
            {
                Success = true,
                Data = new LevelDampakDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("CreateLevelDampak")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id,
            LevelDampakUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.LevelDampaks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level dampak tidak ditemukan" });

            if (await db.LevelDampaks.AnyAsync(l =>
                l.Skor == request.Skor && l.Id != item.Id))
                return Results.BadRequest(new { success = false,
                    message = "Skor sudah digunakan" });

            item.Nama = request.Nama;
            item.Skor = request.Skor;
            item.Deskripsi = request.Deskripsi;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<LevelDampakDto>
            {
                Success = true,
                Data = new LevelDampakDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Skor = item.Skor,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("UpdateLevelDampak")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelDampaks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level dampak tidak ditemukan" });

            if (await db.MatriksRisikos.AnyAsync(m => m.DampakId == item.Id))
                return Results.BadRequest(new { success = false,
                    message = "Level dampak masih digunakan di matriks" });

            db.LevelDampaks.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Level dampak berhasil dihapus" });
        })
        .WithName("DeleteLevelDampak")
        .WithOpenApi();
    }
}

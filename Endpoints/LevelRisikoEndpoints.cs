using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class LevelRisikoEndpoints
{
    public static void MapLevelRisikoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/level-risiko")
            .WithTags("Level Risiko").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber,
            int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.LevelRisikos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(l =>
                    l.Nama.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(l => l.SkorMin)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(l => new LevelRisikoDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Warna = l.Warna,
                    SkorMin = l.SkorMin,
                    SkorMax = l.SkorMax
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<LevelRisikoDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetLevelRisiko")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.LevelRisikos
                .OrderBy(l => l.SkorMin)
                .Select(l => new LevelRisikoDto
                {
                    Id = l.Id.ToString(),
                    Nama = l.Nama,
                    Warna = l.Warna,
                    SkorMin = l.SkorMin,
                    SkorMax = l.SkorMax
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<LevelRisikoDto>>
                { Success = true, Data = items });
        })
        .WithName("GetAllLevelRisiko")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelRisikos.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level risiko tidak ditemukan" });

            return Results.Ok(new ApiResponse<LevelRisikoDto>
            {
                Success = true,
                Data = new LevelRisikoDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Warna = item.Warna,
                    SkorMin = item.SkorMin,
                    SkorMax = item.SkorMax
                }
            });
        })
        .WithName("GetLevelRisikoById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (LevelRisikoCreateRequest request,
            AppDbContext db) =>
        {
            var item = new LevelRisiko
            {
                Nama = request.Nama,
                Warna = request.Warna,
                SkorMin = request.SkorMin,
                SkorMax = request.SkorMax
            };

            db.LevelRisikos.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/level-risiko/{item.Id}",
                new ApiResponse<LevelRisikoDto>
            {
                Success = true,
                Data = new LevelRisikoDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Warna = item.Warna,
                    SkorMin = item.SkorMin,
                    SkorMax = item.SkorMax
                }
            });
        })
        .WithName("CreateLevelRisiko")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id,
            LevelRisikoUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.LevelRisikos.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level risiko tidak ditemukan" });

            item.Nama = request.Nama;
            item.Warna = request.Warna;
            item.SkorMin = request.SkorMin;
            item.SkorMax = request.SkorMax;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<LevelRisikoDto>
            {
                Success = true,
                Data = new LevelRisikoDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Warna = item.Warna,
                    SkorMin = item.SkorMin,
                    SkorMax = item.SkorMax
                }
            });
        })
        .WithName("UpdateLevelRisiko")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.LevelRisikos.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Level risiko tidak ditemukan" });

            if (await db.MatriksRisikos.AnyAsync(m =>
                m.LevelRisikoId == item.Id))
                return Results.BadRequest(new { success = false,
                    message = "Level risiko masih digunakan di matriks" });

            db.LevelRisikos.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Level risiko berhasil dihapus" });
        })
        .WithName("DeleteLevelRisiko")
        .WithOpenApi();
    }
}

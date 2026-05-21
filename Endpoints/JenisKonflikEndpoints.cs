using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class JenisKonflikEndpoints
{
    public static void MapJenisKonflikEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/jenis-konflik").WithTags("Jenis Konflik").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.JenisKonfliks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(j =>
                    j.Nama.ToLower().Contains(search) ||
                    (j.Deskripsi != null && j.Deskripsi.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(j => j.Nama)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(j => new JenisKonflikDto
                {
                    Id = j.Id.ToString(),
                    Nama = j.Nama,
                    Deskripsi = j.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<JenisKonflikDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetJenisKonflik")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.JenisKonfliks
                .OrderBy(j => j.Nama)
                .Select(j => new JenisKonflikDto
                {
                    Id = j.Id.ToString(),
                    Nama = j.Nama,
                    Deskripsi = j.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<JenisKonflikDto>> { Success = true, Data = items });
        })
        .WithName("GetAllJenisKonflik")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.JenisKonfliks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Jenis konflik tidak ditemukan" });

            return Results.Ok(new ApiResponse<JenisKonflikDto>
            {
                Success = true,
                Data = new JenisKonflikDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("GetJenisKonflikById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (JenisKonflikCreateRequest request, AppDbContext db) =>
        {
            if (await db.JenisKonfliks.AnyAsync(j => j.Nama == request.Nama))
                return Results.BadRequest(new { success = false, message = "Nama jenis konflik sudah digunakan" });

            var item = new JenisKonflik
            {
                Nama = request.Nama,
                Deskripsi = request.Deskripsi
            };

            db.JenisKonfliks.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/jenis-konflik/{item.Id}", new ApiResponse<JenisKonflikDto>
            {
                Success = true,
                Data = new JenisKonflikDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("CreateJenisKonflik")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, JenisKonflikUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.JenisKonfliks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Jenis konflik tidak ditemukan" });

            if (await db.JenisKonfliks.AnyAsync(j => j.Nama == request.Nama && j.Id != item.Id))
                return Results.BadRequest(new { success = false, message = "Nama jenis konflik sudah digunakan" });

            item.Nama = request.Nama;
            item.Deskripsi = request.Deskripsi;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<JenisKonflikDto>
            {
                Success = true,
                Data = new JenisKonflikDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("UpdateJenisKonflik")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.JenisKonfliks.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Jenis konflik tidak ditemukan" });

            db.JenisKonfliks.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Jenis konflik berhasil dihapus" });
        })
        .WithName("DeleteJenisKonflik")
        .WithOpenApi();
    }
}

using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class InstansiEndpoints
{
    public static void MapInstansiEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/instansi").WithTags("Instansi").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.Instansis.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(i =>
                    i.Nama.ToLower().Contains(search) ||
                    (i.Deskripsi != null && i.Deskripsi.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(i => i.Nama)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(i => new InstansiDto
                {
                    Id = i.Id.ToString(),
                    Nama = i.Nama,
                    Deskripsi = i.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<InstansiDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetInstansi")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.Instansis
                .OrderBy(i => i.Nama)
                .Select(i => new InstansiDto
                {
                    Id = i.Id.ToString(),
                    Nama = i.Nama,
                    Deskripsi = i.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<InstansiDto>> { Success = true, Data = items });
        })
        .WithName("GetAllInstansi")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.Instansis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Instansi tidak ditemukan" });

            return Results.Ok(new ApiResponse<InstansiDto>
            {
                Success = true,
                Data = new InstansiDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("GetInstansiById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (InstansiCreateRequest request, AppDbContext db) =>
        {
            if (await db.Instansis.AnyAsync(i => i.Nama == request.Nama))
                return Results.BadRequest(new { success = false, message = "Nama instansi sudah digunakan" });

            var item = new Instansi
            {
                Nama = request.Nama,
                Deskripsi = request.Deskripsi
            };

            db.Instansis.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/instansi/{item.Id}", new ApiResponse<InstansiDto>
            {
                Success = true,
                Data = new InstansiDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("CreateInstansi")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, InstansiUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.Instansis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Instansi tidak ditemukan" });

            if (await db.Instansis.AnyAsync(i => i.Nama == request.Nama && i.Id != item.Id))
                return Results.BadRequest(new { success = false, message = "Nama instansi sudah digunakan" });

            item.Nama = request.Nama;
            item.Deskripsi = request.Deskripsi;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<InstansiDto>
            {
                Success = true,
                Data = new InstansiDto
                {
                    Id = item.Id.ToString(),
                    Nama = item.Nama,
                    Deskripsi = item.Deskripsi
                }
            });
        })
        .WithName("UpdateInstansi")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.Instansis.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Instansi tidak ditemukan" });

            db.Instansis.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Instansi berhasil dihapus" });
        })
        .WithName("DeleteInstansi")
        .WithOpenApi();
    }
}

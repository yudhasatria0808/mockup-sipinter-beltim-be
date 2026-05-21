using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class WilayahEndpoints
{
    public static void MapWilayahEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wilayah").WithTags("Wilayah").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, string? tipe, string? parentId, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.Wilayahs.Include(w => w.Parent).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipe))
            {
                query = query.Where(w => w.Tipe == tipe);
            }

            if (!string.IsNullOrWhiteSpace(parentId))
            {
                var pid = Guid.Parse(parentId);
                query = query.Where(w => w.ParentId == pid);
            }

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(w =>
                    w.Nama.ToLower().Contains(search) ||
                    w.KodeBps.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(w => w.KodeBps)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(w => new WilayahDto
                {
                    Id = w.Id.ToString(),
                    Tipe = w.Tipe,
                    Nama = w.Nama,
                    KodeBps = w.KodeBps,
                    ParentId = w.ParentId != null ? w.ParentId.ToString() : null,
                    ParentNama = w.Parent != null ? w.Parent.Nama : null,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<WilayahDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetWilayah")
        .WithOpenApi();

        // GET all (for dropdowns, filtered by tipe)
        group.MapGet("/all", async (string? tipe, string? parentId, AppDbContext db) =>
        {
            var query = db.Wilayahs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipe))
                query = query.Where(w => w.Tipe == tipe);

            if (!string.IsNullOrWhiteSpace(parentId))
            {
                var pid = Guid.Parse(parentId);
                query = query.Where(w => w.ParentId == pid);
            }

            var items = await query
                .OrderBy(w => w.Nama)
                .Select(w => new WilayahDto
                {
                    Id = w.Id.ToString(),
                    Tipe = w.Tipe,
                    Nama = w.Nama,
                    KodeBps = w.KodeBps,
                    ParentId = w.ParentId != null ? w.ParentId.ToString() : null,
                    Latitude = w.Latitude,
                    Longitude = w.Longitude
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<WilayahDto>> { Success = true, Data = items });
        })
        .WithName("GetAllWilayah")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.Wilayahs
                .Include(w => w.Parent)
                .FirstOrDefaultAsync(w => w.Id == Guid.Parse(id));

            if (item is null)
                return Results.NotFound(new { success = false, message = "Wilayah tidak ditemukan" });

            return Results.Ok(new ApiResponse<WilayahDto>
            {
                Success = true,
                Data = new WilayahDto
                {
                    Id = item.Id.ToString(),
                    Tipe = item.Tipe,
                    Nama = item.Nama,
                    KodeBps = item.KodeBps,
                    ParentId = item.ParentId?.ToString(),
                    ParentNama = item.Parent?.Nama,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                }
            });
        })
        .WithName("GetWilayahById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (WilayahCreateRequest request, AppDbContext db) =>
        {
            if (await db.Wilayahs.AnyAsync(w => w.KodeBps == request.KodeBps))
                return Results.BadRequest(new { success = false, message = "Kode BPS sudah digunakan" });

            var item = new Wilayah
            {
                Tipe = request.Tipe,
                Nama = request.Nama,
                KodeBps = request.KodeBps,
                ParentId = !string.IsNullOrWhiteSpace(request.ParentId) ? Guid.Parse(request.ParentId) : null,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            db.Wilayahs.Add(item);
            await db.SaveChangesAsync();

            return Results.Created($"/api/wilayah/{item.Id}", new ApiResponse<WilayahDto>
            {
                Success = true,
                Data = new WilayahDto
                {
                    Id = item.Id.ToString(),
                    Tipe = item.Tipe,
                    Nama = item.Nama,
                    KodeBps = item.KodeBps,
                    ParentId = item.ParentId?.ToString(),
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                }
            });
        })
        .WithName("CreateWilayah")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, WilayahUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.Wilayahs.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Wilayah tidak ditemukan" });

            if (await db.Wilayahs.AnyAsync(w => w.KodeBps == request.KodeBps && w.Id != item.Id))
                return Results.BadRequest(new { success = false, message = "Kode BPS sudah digunakan" });

            item.Tipe = request.Tipe;
            item.Nama = request.Nama;
            item.KodeBps = request.KodeBps;
            item.ParentId = !string.IsNullOrWhiteSpace(request.ParentId) ? Guid.Parse(request.ParentId) : null;
            item.Latitude = request.Latitude;
            item.Longitude = request.Longitude;
            item.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<WilayahDto>
            {
                Success = true,
                Data = new WilayahDto
                {
                    Id = item.Id.ToString(),
                    Tipe = item.Tipe,
                    Nama = item.Nama,
                    KodeBps = item.KodeBps,
                    ParentId = item.ParentId?.ToString(),
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                }
            });
        })
        .WithName("UpdateWilayah")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.Wilayahs.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false, message = "Wilayah tidak ditemukan" });

            // Check if has children
            if (await db.Wilayahs.AnyAsync(w => w.ParentId == item.Id))
                return Results.BadRequest(new { success = false, message = "Wilayah masih memiliki sub-wilayah" });

            db.Wilayahs.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Wilayah berhasil dihapus" });
        })
        .WithName("DeleteWilayah")
        .WithOpenApi();
    }
}

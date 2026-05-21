using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class AspekEndpoints
{
    public static void MapAspekEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/aspek").WithTags("Aspek").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.Aspeks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(a =>
                    a.Nama.ToLower().Contains(search) ||
                    (a.Deskripsi != null && a.Deskripsi.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var items = await query
                .OrderBy(a => a.Nama)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(a => new AspekDto
                {
                    Id = a.Id.ToString(),
                    Nama = a.Nama,
                    Deskripsi = a.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<AspekDto>
            {
                Data = items,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetAspek")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var items = await db.Aspeks
                .OrderBy(a => a.Nama)
                .Select(a => new AspekDto
                {
                    Id = a.Id.ToString(),
                    Nama = a.Nama,
                    Deskripsi = a.Deskripsi
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<AspekDto>> { Success = true, Data = items });
        })
        .WithName("GetAllAspek")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var aspek = await db.Aspeks.FindAsync(Guid.Parse(id));
            if (aspek is null)
                return Results.NotFound(new { success = false, message = "Aspek tidak ditemukan" });

            return Results.Ok(new ApiResponse<AspekDto>
            {
                Success = true,
                Data = new AspekDto
                {
                    Id = aspek.Id.ToString(),
                    Nama = aspek.Nama,
                    Deskripsi = aspek.Deskripsi
                }
            });
        })
        .WithName("GetAspekById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (AspekCreateRequest request, AppDbContext db) =>
        {
            if (await db.Aspeks.AnyAsync(a => a.Nama == request.Nama))
                return Results.BadRequest(new { success = false, message = "Nama aspek sudah digunakan" });

            var aspek = new Aspek
            {
                Nama = request.Nama,
                Deskripsi = request.Deskripsi
            };

            db.Aspeks.Add(aspek);
            await db.SaveChangesAsync();

            return Results.Created($"/api/aspek/{aspek.Id}", new ApiResponse<AspekDto>
            {
                Success = true,
                Data = new AspekDto
                {
                    Id = aspek.Id.ToString(),
                    Nama = aspek.Nama,
                    Deskripsi = aspek.Deskripsi
                }
            });
        })
        .WithName("CreateAspek")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, AspekUpdateRequest request, AppDbContext db) =>
        {
            var aspek = await db.Aspeks.FindAsync(Guid.Parse(id));
            if (aspek is null)
                return Results.NotFound(new { success = false, message = "Aspek tidak ditemukan" });

            if (await db.Aspeks.AnyAsync(a => a.Nama == request.Nama && a.Id != aspek.Id))
                return Results.BadRequest(new { success = false, message = "Nama aspek sudah digunakan" });

            aspek.Nama = request.Nama;
            aspek.Deskripsi = request.Deskripsi;
            aspek.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new ApiResponse<AspekDto>
            {
                Success = true,
                Data = new AspekDto
                {
                    Id = aspek.Id.ToString(),
                    Nama = aspek.Nama,
                    Deskripsi = aspek.Deskripsi
                }
            });
        })
        .WithName("UpdateAspek")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var aspek = await db.Aspeks.FindAsync(Guid.Parse(id));
            if (aspek is null)
                return Results.NotFound(new { success = false, message = "Aspek tidak ditemukan" });

            db.Aspeks.Remove(aspek);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Aspek berhasil dihapus" });
        })
        .WithName("DeleteAspek")
        .WithOpenApi();
    }
}

using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class MatriksRisikoEndpoints
{
    public static void MapMatriksRisikoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/matriks-risiko")
            .WithTags("Matriks Risiko").RequireAuthorization();

        // GET all matriks
        group.MapGet("/", async (AppDbContext db) =>
        {
            var items = await db.MatriksRisikos
                .Include(m => m.Kemungkinan)
                .Include(m => m.Dampak)
                .Include(m => m.LevelRisiko)
                .OrderBy(m => m.Kemungkinan.Skor)
                .ThenBy(m => m.Dampak.Skor)
                .Select(m => new MatriksRisikoDto
                {
                    Id = m.Id.ToString(),
                    KemungkinanId = m.KemungkinanId.ToString(),
                    KemungkinanNama = m.Kemungkinan.Nama,
                    KemungkinanSkor = m.Kemungkinan.Skor,
                    DampakId = m.DampakId.ToString(),
                    DampakNama = m.Dampak.Nama,
                    DampakSkor = m.Dampak.Skor,
                    LevelRisikoId = m.LevelRisikoId.ToString(),
                    LevelRisikoNama = m.LevelRisiko.Nama,
                    LevelRisikoWarna = m.LevelRisiko.Warna
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<MatriksRisikoDto>>
                { Success = true, Data = items });
        })
        .WithName("GetMatriksRisiko")
        .WithOpenApi();

        // POST create single
        group.MapPost("/", async (MatriksRisikoCreateRequest request,
            AppDbContext db) =>
        {
            var kemungkinanId = Guid.Parse(request.KemungkinanId);
            var dampakId = Guid.Parse(request.DampakId);
            var levelRisikoId = Guid.Parse(request.LevelRisikoId);

            // Check if combination already exists
            if (await db.MatriksRisikos.AnyAsync(m =>
                m.KemungkinanId == kemungkinanId &&
                m.DampakId == dampakId))
                return Results.BadRequest(new { success = false,
                    message = "Kombinasi kemungkinan dan dampak sudah ada" });

            var item = new MatriksRisiko
            {
                KemungkinanId = kemungkinanId,
                DampakId = dampakId,
                LevelRisikoId = levelRisikoId
            };

            db.MatriksRisikos.Add(item);
            await db.SaveChangesAsync();

            var created = await db.MatriksRisikos
                .Include(m => m.Kemungkinan)
                .Include(m => m.Dampak)
                .Include(m => m.LevelRisiko)
                .FirstAsync(m => m.Id == item.Id);

            return Results.Created($"/api/matriks-risiko/{item.Id}",
                new ApiResponse<MatriksRisikoDto>
            {
                Success = true,
                Data = new MatriksRisikoDto
                {
                    Id = created.Id.ToString(),
                    KemungkinanId = created.KemungkinanId.ToString(),
                    KemungkinanNama = created.Kemungkinan.Nama,
                    KemungkinanSkor = created.Kemungkinan.Skor,
                    DampakId = created.DampakId.ToString(),
                    DampakNama = created.Dampak.Nama,
                    DampakSkor = created.Dampak.Skor,
                    LevelRisikoId = created.LevelRisikoId.ToString(),
                    LevelRisikoNama = created.LevelRisiko.Nama,
                    LevelRisikoWarna = created.LevelRisiko.Warna
                }
            });
        })
        .WithName("CreateMatriksRisiko")
        .WithOpenApi();

        // POST bulk save (replace all)
        group.MapPost("/bulk", async (MatriksRisikoBulkRequest request,
            AppDbContext db) =>
        {
            // Remove all existing
            var existing = await db.MatriksRisikos.ToListAsync();
            db.MatriksRisikos.RemoveRange(existing);

            // Add new items
            foreach (var item in request.Items)
            {
                db.MatriksRisikos.Add(new MatriksRisiko
                {
                    KemungkinanId = Guid.Parse(item.KemungkinanId),
                    DampakId = Guid.Parse(item.DampakId),
                    LevelRisikoId = Guid.Parse(item.LevelRisikoId)
                });
            }

            await db.SaveChangesAsync();

            var items = await db.MatriksRisikos
                .Include(m => m.Kemungkinan)
                .Include(m => m.Dampak)
                .Include(m => m.LevelRisiko)
                .OrderBy(m => m.Kemungkinan.Skor)
                .ThenBy(m => m.Dampak.Skor)
                .Select(m => new MatriksRisikoDto
                {
                    Id = m.Id.ToString(),
                    KemungkinanId = m.KemungkinanId.ToString(),
                    KemungkinanNama = m.Kemungkinan.Nama,
                    KemungkinanSkor = m.Kemungkinan.Skor,
                    DampakId = m.DampakId.ToString(),
                    DampakNama = m.Dampak.Nama,
                    DampakSkor = m.Dampak.Skor,
                    LevelRisikoId = m.LevelRisikoId.ToString(),
                    LevelRisikoNama = m.LevelRisiko.Nama,
                    LevelRisikoWarna = m.LevelRisiko.Warna
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<MatriksRisikoDto>>
                { Success = true, Data = items,
                  Message = "Matriks risiko berhasil disimpan" });
        })
        .WithName("BulkSaveMatriksRisiko")
        .WithOpenApi();

        // PUT update single
        group.MapPut("/{id}", async (string id,
            MatriksRisikoUpdateRequest request, AppDbContext db) =>
        {
            var item = await db.MatriksRisikos.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Matriks risiko tidak ditemukan" });

            item.KemungkinanId = Guid.Parse(request.KemungkinanId);
            item.DampakId = Guid.Parse(request.DampakId);
            item.LevelRisikoId = Guid.Parse(request.LevelRisikoId);

            await db.SaveChangesAsync();

            var updated = await db.MatriksRisikos
                .Include(m => m.Kemungkinan)
                .Include(m => m.Dampak)
                .Include(m => m.LevelRisiko)
                .FirstAsync(m => m.Id == item.Id);

            return Results.Ok(new ApiResponse<MatriksRisikoDto>
            {
                Success = true,
                Data = new MatriksRisikoDto
                {
                    Id = updated.Id.ToString(),
                    KemungkinanId = updated.KemungkinanId.ToString(),
                    KemungkinanNama = updated.Kemungkinan.Nama,
                    KemungkinanSkor = updated.Kemungkinan.Skor,
                    DampakId = updated.DampakId.ToString(),
                    DampakNama = updated.Dampak.Nama,
                    DampakSkor = updated.Dampak.Skor,
                    LevelRisikoId = updated.LevelRisikoId.ToString(),
                    LevelRisikoNama = updated.LevelRisiko.Nama,
                    LevelRisikoWarna = updated.LevelRisiko.Warna
                }
            });
        })
        .WithName("UpdateMatriksRisiko")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var item = await db.MatriksRisikos.FindAsync(Guid.Parse(id));
            if (item is null)
                return Results.NotFound(new { success = false,
                    message = "Matriks risiko tidak ditemukan" });

            db.MatriksRisikos.Remove(item);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true,
                message = "Matriks risiko berhasil dihapus" });
        })
        .WithName("DeleteMatriksRisiko")
        .WithOpenApi();
    }
}

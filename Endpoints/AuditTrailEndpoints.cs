using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;

namespace SipintarBeltim.Endpoints;

public static class AuditTrailEndpoints
{
    public static void MapAuditTrailEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/audit-trail").WithTags("Audit Trail").RequireAuthorization();

        // GET table names
        group.MapGet("/tables", async (AppDbContext db) =>
        {
            var tables = await db.AuditTrails
                .Select(a => a.TableName)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<string>> { Success = true, Data = tables });
        })
        .WithName("GetAuditTrailTables")
        .WithOpenApi();

        // GET paginated list
        group.MapGet("/", async (
            string? tableName, string? action, string? userId,
            string? fromDate, string? toDate,
            int? page, int? pageSize,
            AppDbContext db) =>
        {
            var p = page ?? 1;
            var size = pageSize ?? 15;
            var query = db.AuditTrails.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tableName))
                query = query.Where(a => a.TableName == tableName);
            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action == action);
            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(a => a.UserId == Guid.Parse(userId));
            if (!string.IsNullOrWhiteSpace(fromDate) && DateTime.TryParse(fromDate, out var from))
                query = query.Where(a => a.Timestamp >= from);
            if (!string.IsNullOrWhiteSpace(toDate) && DateTime.TryParse(toDate, out var to))
                query = query.Where(a => a.Timestamp <= to);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var rawData = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((p - 1) * size)
                .Take(size)
                .ToListAsync();

            var data = rawData.Select(a => new AuditTrailDto
            {
                Id = a.Id.ToString(),
                TableName = a.TableName,
                RecordId = a.RecordId,
                Action = a.Action,
                OldValues = a.OldValues != null ? JsonSerializer.Deserialize<object>(a.OldValues) : null,
                NewValues = a.NewValues != null ? JsonSerializer.Deserialize<object>(a.NewValues) : null,
                ChangedColumns = a.ChangedColumns,
                UserId = a.UserId.ToString(),
                UserName = a.UserName,
                Timestamp = a.Timestamp.ToString("o"),
                IpAddress = a.IpAddress
            }).ToList();

            return Results.Ok(new
            {
                data,
                totalCount,
                page = p,
                pageSize = size,
                totalPages
            });
        })
        .WithName("GetAuditTrails")
        .WithOpenApi();
    }
}

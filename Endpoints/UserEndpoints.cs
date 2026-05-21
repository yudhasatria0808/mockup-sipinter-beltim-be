using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Username.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var users = await query
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(u => new UserListDto
                {
                    Id = u.Id.ToString(),
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    RoleId = u.RoleId.ToString(),
                    RoleName = u.Role.Name,
                    CreatedDate = u.CreatedAt.ToString("o")
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<UserListDto>
            {
                Data = users,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetUsers")
        .WithOpenApi();

        // GET by id
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(id));

            if (user is null)
                return Results.NotFound(new { success = false, message = "User tidak ditemukan" });

            return Results.Ok(new ApiResponse<UserDetailDto>
            {
                Success = true,
                Data = new UserDetailDto
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    RoleId = user.RoleId.ToString(),
                    RoleName = user.Role.Name,
                    CreatedDate = user.CreatedAt.ToString("o"),
                    MustChangePassword = user.MustChangePassword,
                    UpdatedDate = user.UpdatedAt?.ToString("o")
                }
            });
        })
        .WithName("GetUserById")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (UserCreateRequest request, AppDbContext db) =>
        {
            // Check duplicate username
            if (await db.Users.AnyAsync(u => u.Username == request.Username))
                return Results.BadRequest(new { success = false, message = "Username sudah digunakan" });

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                RoleId = Guid.Parse(request.RoleId),
                IsActive = request.IsActive,
                MustChangePassword = true
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var created = await db.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id);

            return Results.Created($"/api/users/{user.Id}", new ApiResponse<UserDetailDto>
            {
                Success = true,
                Data = new UserDetailDto
                {
                    Id = created.Id.ToString(),
                    Username = created.Username,
                    FullName = created.FullName,
                    Email = created.Email,
                    IsActive = created.IsActive,
                    RoleId = created.RoleId.ToString(),
                    RoleName = created.Role.Name,
                    CreatedDate = created.CreatedAt.ToString("o"),
                    MustChangePassword = created.MustChangePassword,
                    UpdatedDate = created.UpdatedAt?.ToString("o")
                }
            });
        })
        .WithName("CreateUser")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, UserUpdateRequest request, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(Guid.Parse(id));
            if (user is null)
                return Results.NotFound(new { success = false, message = "User tidak ditemukan" });

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.RoleId = Guid.Parse(request.RoleId);
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            var updated = await db.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id);

            return Results.Ok(new ApiResponse<UserDetailDto>
            {
                Success = true,
                Data = new UserDetailDto
                {
                    Id = updated.Id.ToString(),
                    Username = updated.Username,
                    FullName = updated.FullName,
                    Email = updated.Email,
                    IsActive = updated.IsActive,
                    RoleId = updated.RoleId.ToString(),
                    RoleName = updated.Role.Name,
                    CreatedDate = updated.CreatedAt.ToString("o"),
                    MustChangePassword = updated.MustChangePassword,
                    UpdatedDate = updated.UpdatedAt?.ToString("o")
                }
            });
        })
        .WithName("UpdateUser")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(Guid.Parse(id));
            if (user is null)
                return Results.NotFound(new { success = false, message = "User tidak ditemukan" });

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "User berhasil dihapus" });
        })
        .WithName("DeleteUser")
        .WithOpenApi();

        // POST toggle status
        group.MapPost("/{id}/toggle-status", async (string id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(Guid.Parse(id));
            if (user is null)
                return Results.NotFound(new { success = false, message = "User tidak ditemukan" });

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = $"Status user berhasil diubah menjadi {(user.IsActive ? "aktif" : "nonaktif")}" });
        })
        .WithName("ToggleUserStatus")
        .WithOpenApi();

        // POST reset password
        group.MapPost("/{id}/reset-password", async (string id, UserResetPasswordRequest request, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(Guid.Parse(id));
            if (user is null)
                return Results.NotFound(new { success = false, message = "User tidak ditemukan" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.MustChangePassword = true;
            user.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Password berhasil direset" });
        })
        .WithName("ResetUserPassword")
        .WithOpenApi();
    }
}

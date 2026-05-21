using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;

namespace SipintarBeltim.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, AppDbContext db, IConfiguration config) =>
        {
            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Results.Unauthorized();

            if (!user.IsActive)
                return Results.Json(new { success = false, message = "Akun tidak aktif" }, statusCode: 403);

            var token = GenerateJwtToken(user, config);

            return Results.Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = new LoginResponse
                {
                    Token = token,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = user.Role.Name,
                    RoleId = user.RoleId.ToString(),
                    UserId = user.Id.ToString()
                }
            });
        })
        .WithName("Login")
        .AllowAnonymous()
        .WithOpenApi();

        group.MapGet("/me", async (ClaimsPrincipal claims, AppDbContext db) =>
        {
            var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            if (user is null)
                return Results.Unauthorized();

            // Get permissions
            var permissions = await db.RolePermissions
                .Include(rp => rp.Menu)
                .Where(rp => rp.RoleId == user.RoleId)
                .ToListAsync();

            var menuPermissions = permissions.Select(p => new MenuPermissionDto
            {
                MenuId = p.MenuId.ToString(),
                MenuName = p.Menu.Name,
                MenuDescription = p.Menu.Description ?? "",
                CanView = p.CanView,
                CanCreate = p.CanCreate,
                CanUpdate = p.CanUpdate,
                CanDelete = p.CanDelete,
                HasAccess = p.CanView || p.CanCreate || p.CanUpdate || p.CanDelete,
                Idx = p.Menu.Idx
            }).ToList();

            return Results.Ok(new ApiResponse<MeResponse>
            {
                Success = true,
                Data = new MeResponse
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = user.Role.Name,
                    RoleId = user.RoleId.ToString(),
                    Permissions = menuPermissions
                }
            });
        })
        .WithName("GetMe")
        .RequireAuthorization()
        .WithOpenApi();
    }

    private static string GenerateJwtToken(Models.User user, IConfiguration config)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? "SipintarBeltimSecretKey2024VeryLongKeyForSecurity!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("roleId", user.RoleId.ToString()),
            new Claim("fullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"] ?? "SipintarBeltim",
            audience: config["Jwt:Audience"] ?? "SipintarBeltimApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

using Microsoft.EntityFrameworkCore;
using SipintarBeltim.Data;
using SipintarBeltim.DTOs;
using SipintarBeltim.Models;

namespace SipintarBeltim.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/roles").WithTags("Roles").RequireAuthorization();

        // GET paginated
        group.MapGet("/", async (string? generalSearch, int? pageNumber, int? pageSize, AppDbContext db) =>
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var query = db.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(generalSearch))
            {
                var search = generalSearch.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var roles = await query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(r => new RoleListDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    Description = r.Description,
                    IsProtected = r.IsProtected
                })
                .ToListAsync();

            return Results.Ok(new PaginatedResponse<RoleListDto>
            {
                Data = roles,
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalCount = totalCount
            });
        })
        .WithName("GetRoles")
        .WithOpenApi();

        // GET all (for dropdowns)
        group.MapGet("/all", async (AppDbContext db) =>
        {
            var roles = await db.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleListDto
                {
                    Id = r.Id.ToString(),
                    Name = r.Name,
                    Description = r.Description,
                    IsProtected = r.IsProtected
                })
                .ToListAsync();

            return Results.Ok(new ApiResponse<List<RoleListDto>> { Success = true, Data = roles });
        })
        .WithName("GetAllRoles")
        .WithOpenApi();

        // GET by id (with permissions)
        group.MapGet("/{id}", async (string id, AppDbContext db) =>
        {
            var role = await db.Roles.FindAsync(Guid.Parse(id));
            if (role is null)
                return Results.NotFound(new { success = false, message = "Role tidak ditemukan" });

            var permissions = await GetRolePermissionStructure(Guid.Parse(id), db);

            return Results.Ok(new ApiResponse<RoleDetailDto>
            {
                Success = true,
                Data = new RoleDetailDto
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Description = role.Description,
                    IsProtected = role.IsProtected,
                    RolePermission = permissions
                }
            });
        })
        .WithName("GetRoleById")
        .WithOpenApi();

        // GET default permissions (for create form)
        group.MapGet("/default-permissions", async (AppDbContext db) =>
        {
            var permissions = await GetDefaultPermissionStructure(db);
            return Results.Ok(new ApiResponse<List<RolePermissionModulDto>> { Success = true, Data = permissions });
        })
        .WithName("GetDefaultPermissions")
        .WithOpenApi();

        // POST create
        group.MapPost("/", async (RoleCreateRequest request, AppDbContext db) =>
        {
            if (await db.Roles.AnyAsync(r => r.Name == request.Name))
                return Results.BadRequest(new { success = false, message = "Nama role sudah digunakan" });

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description
            };
            db.Roles.Add(role);
            await db.SaveChangesAsync();

            // Save permissions
            await SaveRolePermissions(role.Id, request.RolePermission, db);

            var permissions = await GetRolePermissionStructure(role.Id, db);

            return Results.Created($"/api/roles/{role.Id}", new ApiResponse<RoleDetailDto>
            {
                Success = true,
                Data = new RoleDetailDto
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Description = role.Description,
                    IsProtected = role.IsProtected,
                    RolePermission = permissions
                }
            });
        })
        .WithName("CreateRole")
        .WithOpenApi();

        // PUT update
        group.MapPut("/{id}", async (string id, RoleUpdateRequest request, AppDbContext db) =>
        {
            var role = await db.Roles.FindAsync(Guid.Parse(id));
            if (role is null)
                return Results.NotFound(new { success = false, message = "Role tidak ditemukan" });

            if (role.IsProtected)
                return Results.BadRequest(new { success = false, message = "Role yang dilindungi tidak dapat diubah" });

            // Check duplicate name
            if (await db.Roles.AnyAsync(r => r.Name == request.Name && r.Id != role.Id))
                return Results.BadRequest(new { success = false, message = "Nama role sudah digunakan" });

            role.Name = request.Name;
            role.Description = request.Description;

            // Remove old permissions and save new ones
            var oldPermissions = await db.RolePermissions.Where(rp => rp.RoleId == role.Id).ToListAsync();
            db.RolePermissions.RemoveRange(oldPermissions);
            await db.SaveChangesAsync();

            await SaveRolePermissions(role.Id, request.RolePermission, db);

            var permissions = await GetRolePermissionStructure(role.Id, db);

            return Results.Ok(new ApiResponse<RoleDetailDto>
            {
                Success = true,
                Data = new RoleDetailDto
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    Description = role.Description,
                    IsProtected = role.IsProtected,
                    RolePermission = permissions
                }
            });
        })
        .WithName("UpdateRole")
        .WithOpenApi();

        // DELETE
        group.MapDelete("/{id}", async (string id, AppDbContext db) =>
        {
            var role = await db.Roles.FindAsync(Guid.Parse(id));
            if (role is null)
                return Results.NotFound(new { success = false, message = "Role tidak ditemukan" });

            if (role.IsProtected)
                return Results.BadRequest(new { success = false, message = "Role yang dilindungi tidak dapat dihapus" });

            // Check if role is in use
            if (await db.Users.AnyAsync(u => u.RoleId == role.Id))
                return Results.BadRequest(new { success = false, message = "Role masih digunakan oleh user" });

            db.Roles.Remove(role);
            await db.SaveChangesAsync();

            return Results.Ok(new { success = true, message = "Role berhasil dihapus" });
        })
        .WithName("DeleteRole")
        .WithOpenApi();
    }

    private static async Task SaveRolePermissions(Guid roleId, List<RolePermissionRequest> permissions, AppDbContext db)
    {
        foreach (var modulPerm in permissions)
        {
            foreach (var menuPerm in modulPerm.Menus)
            {
                if (Guid.TryParse(menuPerm.MenuId, out var menuId))
                {
                    db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = roleId,
                        MenuId = menuId,
                        CanView = menuPerm.CanView,
                        CanCreate = menuPerm.CanCreate,
                        CanUpdate = menuPerm.CanUpdate,
                        CanDelete = menuPerm.CanDelete,
                        CanApprove = menuPerm.CanApprove
                    });
                }
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task<List<RolePermissionModulDto>> GetRolePermissionStructure(Guid roleId, AppDbContext db)
    {
        var moduls = await db.Moduls
            .Include(m => m.Menus.Where(menu => menu.ParentId == null))
                .ThenInclude(menu => menu.Children)
            .OrderBy(m => m.Idx)
            .ToListAsync();

        var rolePerms = await db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToDictionaryAsync(rp => rp.MenuId);

        return moduls.Select(modul => new RolePermissionModulDto
        {
            ModulId = modul.Id.ToString(),
            ModulName = modul.Name,
            ModulDescription = modul.Description ?? "",
            Idx = modul.Idx,
            Menus = modul.Menus.Where(m => m.ParentId == null).OrderBy(m => m.Idx).Select(menu =>
            {
                rolePerms.TryGetValue(menu.Id, out var perm);
                return new MenuPermissionDto
                {
                    MenuId = menu.Id.ToString(),
                    MenuName = menu.Name,
                    MenuDescription = menu.Description ?? "",
                    CanView = perm?.CanView ?? false,
                    CanCreate = perm?.CanCreate ?? false,
                    CanUpdate = perm?.CanUpdate ?? false,
                    CanDelete = perm?.CanDelete ?? false,
                    CanApprove = perm?.CanApprove ?? false,
                    HasAccess = (perm?.CanView ?? false) || (perm?.CanCreate ?? false) || (perm?.CanUpdate ?? false) || (perm?.CanDelete ?? false),
                    Idx = menu.Idx,
                    Child = menu.Children.OrderBy(c => c.Idx).Select(child =>
                    {
                        rolePerms.TryGetValue(child.Id, out var childPerm);
                        return new MenuPermissionDto
                        {
                            MenuId = child.Id.ToString(),
                            MenuName = child.Name,
                            MenuDescription = child.Description ?? "",
                            CanView = childPerm?.CanView ?? false,
                            CanCreate = childPerm?.CanCreate ?? false,
                            CanUpdate = childPerm?.CanUpdate ?? false,
                            CanDelete = childPerm?.CanDelete ?? false,
                            CanApprove = childPerm?.CanApprove ?? false,
                            HasAccess = (childPerm?.CanView ?? false) || (childPerm?.CanCreate ?? false) || (childPerm?.CanUpdate ?? false) || (childPerm?.CanDelete ?? false),
                            Idx = child.Idx,
                            Child = new List<MenuPermissionDto>()
                        };
                    }).ToList()
                };
            }).ToList()
        }).ToList();
    }

    private static async Task<List<RolePermissionModulDto>> GetDefaultPermissionStructure(AppDbContext db)
    {
        var moduls = await db.Moduls
            .Include(m => m.Menus.Where(menu => menu.ParentId == null))
                .ThenInclude(menu => menu.Children)
            .OrderBy(m => m.Idx)
            .ToListAsync();

        return moduls.Select(modul => new RolePermissionModulDto
        {
            ModulId = modul.Id.ToString(),
            ModulName = modul.Name,
            ModulDescription = modul.Description ?? "",
            Idx = modul.Idx,
            Menus = modul.Menus.Where(m => m.ParentId == null).OrderBy(m => m.Idx).Select(menu => new MenuPermissionDto
            {
                MenuId = menu.Id.ToString(),
                MenuName = menu.Name,
                MenuDescription = menu.Description ?? "",
                CanView = false,
                CanCreate = false,
                CanUpdate = false,
                CanDelete = false,
                CanApprove = false,
                HasAccess = false,
                Idx = menu.Idx,
                Child = menu.Children.OrderBy(c => c.Idx).Select(child => new MenuPermissionDto
                {
                    MenuId = child.Id.ToString(),
                    MenuName = child.Name,
                    MenuDescription = child.Description ?? "",
                    CanView = false,
                    CanCreate = false,
                    CanUpdate = false,
                    CanDelete = false,
                    CanApprove = false,
                    HasAccess = false,
                    Idx = child.Idx,
                    Child = new List<MenuPermissionDto>()
                }).ToList()
            }).ToList()
        }).ToList();
    }
}

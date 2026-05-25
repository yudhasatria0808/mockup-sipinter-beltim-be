namespace SipintarBeltim.DTOs;

public class RoleListDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsProtected { get; set; }
}

public class RoleDetailDto : RoleListDto
{
    public List<RolePermissionModulDto> RolePermission { get; set; } = new();
}

public class RolePermissionModulDto
{
    public string ModulId { get; set; } = string.Empty;
    public string ModulName { get; set; } = string.Empty;
    public string ModulDescription { get; set; } = string.Empty;
    public int Idx { get; set; }
    public List<MenuPermissionDto> Menus { get; set; } = new();
}

public class MenuPermissionDto
{
    public string MenuId { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string MenuDescription { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanApprove { get; set; }
    public bool HasAccess { get; set; }
    public int Idx { get; set; }
    public List<MenuPermissionDto> Child { get; set; } = new();
}

public class RoleCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<RolePermissionRequest> RolePermission { get; set; } = new();
}

public class RoleUpdateRequest : RoleCreateRequest { }

public class RolePermissionRequest
{
    public string ModulId { get; set; } = string.Empty;
    public List<RoleMenuPermissionRequest> Menus { get; set; } = new();
}

public class RoleMenuPermissionRequest
{
    public string MenuId { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanApprove { get; set; }
}

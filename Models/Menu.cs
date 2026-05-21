namespace SipintarBeltim.Models;

public class Menu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Idx { get; set; }
    public Guid ModulId { get; set; }
    public Modul Modul { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public Menu? Parent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Menu> Children { get; set; } = new List<Menu>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

namespace SipintarBeltim.Models;

public class Modul
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Idx { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
}

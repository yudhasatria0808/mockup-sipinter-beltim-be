namespace SipintarBeltim.Models;

public class MatriksRisiko
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KemungkinanId { get; set; }
    public LevelKemungkinan Kemungkinan { get; set; } = null!;
    public Guid DampakId { get; set; }
    public LevelDampak Dampak { get; set; } = null!;
    public Guid LevelRisikoId { get; set; }
    public LevelRisiko LevelRisiko { get; set; } = null!;
}

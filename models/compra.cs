namespace TravelAI.Models;

public class Compra
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int ViagemId { get; set; }

    public DateTime DataCompra { get; set; } = DateTime.UtcNow;

    public Usuario Usuario { get; set; } = null!;

    public Viagem Viagem { get; set; } = null!;
}
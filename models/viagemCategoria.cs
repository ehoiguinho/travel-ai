namespace TravelAI.Models;

public class ViagemCategoria
{
    public int ViagemId { get; set; }

    public int CategoriaId { get; set; }

    public Viagem Viagem { get; set; } = null!;

    public Categoria Categoria { get; set; } = null!;
}
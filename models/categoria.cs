namespace TravelAI.Models;

public class Categoria
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public ICollection<ViagemCategoria> ViagemCategorias { get; set; } = new List<ViagemCategoria>();
}
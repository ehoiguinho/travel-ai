namespace TravelAI.Models;

public class Viagem
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Pais { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal Preco { get; set; }

    public int DuracaoDias { get; set; }

    public ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public ICollection<ViagemCategoria> ViagemCategorias { get; set; } = new List<ViagemCategoria>();
};
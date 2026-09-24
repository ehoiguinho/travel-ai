namespace TravelAI.Models;

public class KnowledgeSearchResult
{
    public int Id { get; set; }

    public int ViagemId { get; set; }

    public string Content { get; set; } = string.Empty;

    public decimal Similaridade { get; set; }

    public Viagem Viagem { get; set; } = null!;
}
using Pgvector;

namespace TravelAI.Models;

public class KnowledgeChunk
{
    public int Id { get; set; }

    public int ViagemId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Vector? Embedding { get; set; }

    public Viagem Viagem { get; set; } = null!;
}
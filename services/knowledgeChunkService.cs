using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Services;

public class KnowledgeService
{
    private readonly AppDbContext _context;
    private readonly EmbeddingService _embeddingService;

    public KnowledgeService(
        AppDbContext context,
        EmbeddingService embeddingService)
    {
        _context = context;
        _embeddingService = embeddingService;
    }

    public async Task<int> IndexarViagensAsync()
    {
        var viagens = await _context.Viagens
            .Include(v => v.ViagemCategorias)
                .ThenInclude(vc => vc.Categoria)
            .AsNoTracking()
            .ToListAsync();

        foreach (var viagem in viagens)
        {
            var categorias = string.Join(
                ", ",
                viagem.ViagemCategorias
                    .Select(vc => vc.Categoria.Nome)
            );

            var conteudo = $"""
                Nome: {viagem.Nome}
                País: {viagem.Pais}
                Cidade: {viagem.Cidade}
                Descrição: {viagem.Descricao}
                Duração: {viagem.DuracaoDias} dias
                Preço: R$ {viagem.Preco:F2}
                Categorias: {categorias}
                """;

            var embedding =
                await _embeddingService
                    .GerarEmbeddingDocumentoAsync(conteudo);

            var chunkExistente = await _context.KnowledgeChunks
                .FirstOrDefaultAsync(k => k.ViagemId == viagem.Id);

            if (chunkExistente != null)
            {
                chunkExistente.Content = conteudo;
                chunkExistente.Embedding = embedding;
            }
            else
            {
                var chunk = new KnowledgeChunk
                {
                    ViagemId = viagem.Id,
                    Content = conteudo,
                    Embedding = embedding
                };

                _context.KnowledgeChunks.Add(chunk);
            }
        }

        await _context.SaveChangesAsync();

        return viagens.Count;
    }

        public async Task<List<KnowledgeChunk>> BuscarAsync(
        string consulta,
        int quantidade = 5)
    {
        var embeddingConsulta =
            await _embeddingService
                .GerarEmbeddingConsultaAsync(consulta);

        var resultados = await _context.KnowledgeChunks
            .Include(k => k.Viagem)
            .OrderBy(k => k.Embedding!.CosineDistance(embeddingConsulta))
            .Take(quantidade)
            .AsNoTracking()
            .ToListAsync();

        return resultados;
    }
}
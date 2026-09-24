using Microsoft.AspNetCore.Mvc;
using TravelAI.Services;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly KnowledgeService _knowledgeService;
    private readonly GenerationService _generationService;

    public ChatController(
        KnowledgeService knowledgeService,
        GenerationService generationService)
    {
        _knowledgeService = knowledgeService;
        _generationService = generationService;
    }

    [HttpPost("perguntar")]
    public async Task<IActionResult> Perguntar(
        [FromBody] string pergunta)
    {
        if (string.IsNullOrWhiteSpace(pergunta))
        {
            return BadRequest(new
            {
                mensagem = "A pergunta é obrigatória."
            });
        }

        var resultados =
            await _knowledgeService.BuscarAsync(
                pergunta,
                5
            );

        if (resultados.Count == 0)
        {
            return Ok(new
            {
                pergunta,
                resposta = "Não encontrei informações suficientes sobre esse assunto no catálogo de viagens.",
                fontes = Array.Empty<object>()
            });
        }

        var contexto = string.Join(
            "\n\n---\n\n",
            resultados.Select(r => r.Content)
        );

        var resposta =
            await _generationService.GerarRespostaAsync(
                pergunta,
                contexto
            );

        return Ok(new
        {
            pergunta,
            resposta,
            fontes = resultados.Select(r => new
            {
                r.ViagemId,
                viagem = r.Viagem!.Nome,
                similaridade = Math.Round(r.Similaridade, 4)
            })
        });
    }
}
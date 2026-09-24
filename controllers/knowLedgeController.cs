using Microsoft.AspNetCore.Mvc;
using TravelAI.Services;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KnowledgeController : ControllerBase
{
    private readonly KnowledgeService _knowledgeService;

    public KnowledgeController(KnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    [HttpPost("indexar")]
    public async Task<IActionResult> Indexar()
    {
        var quantidade =
            await _knowledgeService.IndexarViagensAsync();

        return Ok(new
        {
            mensagem = "Viagens indexadas com sucesso.",
            quantidade
        });
    }

    [HttpPost("buscar")]
    public async Task<IActionResult> Buscar(
        [FromBody] string consulta)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest(new
            {
                mensagem = "A consulta é obrigatória."
            });
        }

        var resultados =
            await _knowledgeService.BuscarAsync(consulta);

        return Ok(resultados.Select(k => new
        {
            k.Id,
            k.ViagemId,
            viagem = new
            {
                k.Viagem!.Id,
                k.Viagem.Nome,
                k.Viagem.Pais,
                k.Viagem.Cidade
            },
            k.Content,
            similaridade = Math.Round(k.Similaridade, 4)
        }));
    }
}
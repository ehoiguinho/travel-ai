using Microsoft.AspNetCore.Mvc;
using TravelAI.Services;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingController : ControllerBase
{
    private readonly EmbeddingService _embeddingService;

    public EmbeddingController(EmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    [HttpPost("teste")]
    public async Task<IActionResult> TestarEmbedding(
        [FromBody] string texto)
    {
        var embedding = await _embeddingService.GerarEmbeddingDocumentoAsync(texto);

        return Ok(new
        {
            dimensoes = embedding.ToArray().Length,
            embedding = embedding.ToArray()
        });
    }
}
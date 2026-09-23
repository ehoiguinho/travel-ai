using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViagemController : ControllerBase
{
    private readonly AppDbContext _context;

    public ViagemController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Viagem>>> GetViagens()
    {
        var viagens = await _context.Viagens
            .AsNoTracking()
            .ToListAsync();

        return Ok(viagens);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetViagem(int id)
    {
        var viagem = await _context.Viagens
            .Include(v => v.ViagemCategorias)
                .ThenInclude(vc => vc.Categoria)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (viagem == null)
        {
            return NotFound(new { mensagem = "Viagem não encontrada." });
        }

        var resultado = new
        {
            viagem.Id,
            viagem.Nome,
            viagem.Pais,
            viagem.Cidade,
            viagem.Descricao,
            viagem.Preco,
            viagem.DuracaoDias,

            Categorias = viagem.ViagemCategorias
                .Select(vc => new
                {
                    vc.Categoria.Id,
                    vc.Categoria.Nome
                })
                .ToList()
        };

        return Ok(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<Viagem>> CriarViagem([FromBody] Viagem viagem)
    {
        if (string.IsNullOrWhiteSpace(viagem.Nome))
        {
            return BadRequest(new { mensagem = "O nome da viagem é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Pais))
        {
            return BadRequest(new { mensagem = "O país é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Cidade))
        {
            return BadRequest(new { mensagem = "A cidade é obrigatória." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Descricao))
        {
            return BadRequest(new { mensagem = "A descrição é obrigatória." });
        }

        if (viagem.Preco <= 0)
        {
            return BadRequest(new { mensagem = "O preço deve ser maior que zero." });
        }

        if (viagem.DuracaoDias <= 0)
        {
            return BadRequest(new { mensagem = "A duração deve ser maior que zero." });
        }

        viagem.Nome = viagem.Nome.Trim();
        viagem.Pais = viagem.Pais.Trim();
        viagem.Cidade = viagem.Cidade.Trim();
        viagem.Descricao = viagem.Descricao.Trim();

        _context.Viagens.Add(viagem);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetViagens),
            new { id = viagem.Id },
            viagem
        );
    }

    [HttpPost("{viagemId}/categorias/{categoriaId}")]
    public async Task<IActionResult> AdicionarCategoria(
        int viagemId,
        int categoriaId)
    {
        var viagem = await _context.Viagens
            .FirstOrDefaultAsync(v => v.Id == viagemId);

        if (viagem == null)
        {
            return NotFound(new { mensagem = "Viagem não encontrada." });
        }

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == categoriaId);

        if (categoria == null)
        {
            return NotFound(new { mensagem = "Categoria não encontrada." });
        }

        var relacionamentoExiste = await _context.ViagemCategorias
            .AnyAsync(vc =>
                vc.ViagemId == viagemId &&
                vc.CategoriaId == categoriaId);

        if (relacionamentoExiste)
        {
            return Conflict(new
            {
                mensagem = "Essa categoria já está associada à viagem."
            });
        }

        var viagemCategoria = new ViagemCategoria
        {
            ViagemId = viagemId,
            CategoriaId = categoriaId
        };

        _context.ViagemCategorias.Add(viagemCategoria);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Categoria adicionada à viagem com sucesso."
        }); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarViagem(int id,[FromBody] Viagem viagem)
    {
        var viagemExistente = await _context.Viagens
            .FirstOrDefaultAsync(v => v.Id == id);

        if (viagemExistente == null)
        {
            return NotFound(new { mensagem = "Viagem não encontrada." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Nome))
        {
            return BadRequest(new { mensagem = "O nome da viagem é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Pais))
        {
            return BadRequest(new { mensagem = "O país é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Cidade))
        {
            return BadRequest(new { mensagem = "A cidade é obrigatória." });
        }

        if (string.IsNullOrWhiteSpace(viagem.Descricao))
        {
            return BadRequest(new { mensagem = "A descrição é obrigatória." });
        }

        if (viagem.Preco <= 0)
        {
            return BadRequest(new { mensagem = "O preço deve ser maior que zero." });
        }

        if (viagem.DuracaoDias <= 0)
        {
            return BadRequest(new { mensagem = "A duração deve ser maior que zero." });
        }

            viagemExistente.Nome = viagem.Nome.Trim();
            viagemExistente.Pais = viagem.Pais.Trim();
            viagemExistente.Cidade = viagem.Cidade.Trim();
            viagemExistente.Descricao = viagem.Descricao.Trim();
            viagemExistente.Preco = viagem.Preco;
            viagemExistente.DuracaoDias = viagem.DuracaoDias;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarViagem(int id)
    {
        var viagem = await _context.Viagens
            .FirstOrDefaultAsync(v => v.Id == id);

        if (viagem == null)
        {
            return NotFound(new { mensagem = "Viagem não encontrada." });
        }

        _context.Viagens.Remove(viagem);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categoria
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
    {
        var categorias = await _context.Categorias
            .AsNoTracking()
            .ToListAsync();

        return Ok(categorias);
    }

    // GET: api/categoria/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetCategoria(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound(new
            {
                mensagem = "Categoria não encontrada."
            });
        }

        return Ok(categoria);
    }

    // POST: api/categoria
    [HttpPost]
    public async Task<ActionResult<Categoria>> CriarCategoria(
        [FromBody] Categoria categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome da categoria é obrigatório."
            });
        }

        var existe = await _context.Categorias
            .AnyAsync(c => c.Nome.ToLower() == categoria.Nome.ToLower());

        if (existe)
        {
            return Conflict(new
            {
                mensagem = "Já existe uma categoria com esse nome."
            });
        }

        categoria.Nome = categoria.Nome.Trim();

        _context.Categorias.Add(categoria);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategoria),
            new { id = categoria.Id },
            categoria
        );
    }

    // PUT: api/categoria/1
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarCategoria(
        int id,
        [FromBody] Categoria categoria)
    {
        var categoriaExistente = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoriaExistente == null)
        {
            return NotFound(new
            {
                mensagem = "Categoria não encontrada."
            });
        }

        if (string.IsNullOrWhiteSpace(categoria.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome da categoria é obrigatório."
            });
        }

        var nomeExiste = await _context.Categorias
            .AnyAsync(c =>
                c.Id != id &&
                c.Nome.ToLower() == categoria.Nome.ToLower());

        if (nomeExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe outra categoria com esse nome."
            });
        }

        categoriaExistente.Nome = categoria.Nome.Trim();

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/categoria/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarCategoria(int id)
    {
        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria == null)
        {
            return NotFound(new
            {
                mensagem = "Categoria não encontrada."
            });
        }

        _context.Categorias.Remove(categoria);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
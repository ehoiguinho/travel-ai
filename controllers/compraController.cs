using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompraController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompraController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CriarCompra(
        [FromBody] Compra compra)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == compra.UsuarioId);

        if (!usuarioExiste)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        var viagemExiste = await _context.Viagens
            .AnyAsync(v => v.Id == compra.ViagemId);

        if (!viagemExiste)
        {
            return NotFound(new
            {
                mensagem = "Viagem não encontrada."
            });
        }

        compra.DataCompra = DateTime.UtcNow;

        _context.Compras.Add(compra);

        await _context.SaveChangesAsync();

        return Ok(compra);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> GetComprasUsuario(int usuarioId)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == usuarioId);

        if (!usuarioExiste)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        var compras = await _context.Compras
            .Where(c => c.UsuarioId == usuarioId)
            .Include(c => c.Viagem)
            .ThenInclude(v => v.ViagemCategorias)
            .ThenInclude(vc => vc.Categoria)
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.UsuarioId,
                c.ViagemId,
                c.DataCompra,

                Viagem = new
                {
                    c.Viagem!.Id,
                    c.Viagem.Nome,
                    c.Viagem.Pais,
                    c.Viagem.Cidade,
                    c.Viagem.Descricao,
                    c.Viagem.Preco,
                    c.Viagem.DuracaoDias,

                    Categorias = c.Viagem.ViagemCategorias
                    .Select(vc => new
                    {
                        vc.Categoria.Id,
                        vc.Categoria.Nome
                        
                    }).ToList()
                }
            }).ToListAsync();

        return Ok(compras);
    }
}
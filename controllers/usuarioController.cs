using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAI.Data;
using TravelAI.Models;

namespace TravelAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> CriarUsuario(
        [FromBody] Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome do usuário é obrigatório."
            });
        }

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            return BadRequest(new
            {
                mensagem = "O email do usuário é obrigatório."
            });
        }

        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim();

        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == usuario.Email.ToLower());

        if (emailExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe um usuário com esse email."
            });
        }

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUsuario),
            new { id = usuario.Id },
            usuario
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] Usuario usuario)
    {
        var usuarioExistente = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuarioExistente == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        if (string.IsNullOrWhiteSpace(usuario.Nome))
        {
            return BadRequest(new
            {
                mensagem = "O nome do usuário é obrigatório."
            });
        }

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            return BadRequest(new
            {
                mensagem = "O email do usuário é obrigatório."
            });
        }

        usuario.Nome = usuario.Nome.Trim();
        usuario.Email = usuario.Email.Trim();

        var emailExiste = await _context.Usuarios
            .AnyAsync(u =>
                u.Id != id &&
                u.Email.ToLower() == usuario.Email.ToLower());

        if (emailExiste)
        {
            return Conflict(new
            {
                mensagem = "Já existe outro usuário com esse email."
            });
        }

        usuarioExistente.Nome = usuario.Nome;
        usuarioExistente.Email = usuario.Email;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarUsuario(int id)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        _context.Usuarios.Remove(usuario);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/interesses")]
    public async Task<IActionResult> GetInteressesUsuario(int id)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == id);

        if (!usuarioExiste)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        var interesses = await _context.Compras
            .Where(c => c.UsuarioId == id)
            .SelectMany(c => c.Viagem.ViagemCategorias)
            .GroupBy(vc => new
            {
                vc.CategoriaId,
                vc.Categoria.Nome
            })
            .Select(g => new
            {
                categoriaId = g.Key.CategoriaId,
                categoria = g.Key.Nome,
                pontuacao = g.Count()
            })
            .OrderByDescending(x => x.pontuacao)
            .ToListAsync();

        return Ok(new
        {
            usuarioId = id,
            interesses
        });
    }

    [HttpGet("{id}/recomendacoes")]
    public async Task<IActionResult> GetRecomendacoesUsuario(int id)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == id);

        if (!usuarioExiste)
        {
            return NotFound(new
            {
                mensagem = "Usuário não encontrado."
            });
        }

        // 1. Descobre os interesses do usuário
        var interesses = await _context.Compras
            .Where(c => c.UsuarioId == id)
            .SelectMany(c => c.Viagem.ViagemCategorias)
            .GroupBy(vc => vc.CategoriaId)
            .Select(g => new
            {
                CategoriaId = g.Key,
                Pontuacao = g.Count()
            })
            .ToListAsync();

        // 2. Descobre quais viagens o usuário já comprou
        var viagensCompradas = await _context.Compras
            .Where(c => c.UsuarioId == id)
            .Select(c => c.ViagemId)
            .ToListAsync();

        // 3. Busca viagens que ainda podem ser recomendadas
        var viagens = await _context.Viagens
            .Include(v => v.ViagemCategorias)
                .ThenInclude(vc => vc.Categoria)
            .Where(v => !viagensCompradas.Contains(v.Id))
            .AsNoTracking()
            .ToListAsync();

        // 4. Calcula a pontuação de cada viagem
        var recomendacoes = viagens
        .Select(v => new
        {
            v.Id,
            v.Nome,
            v.Pais,
            v.Cidade,
            v.Descricao,
            v.Preco,
            v.DuracaoDias,

            Pontuacao = v.ViagemCategorias
                .Sum(vc =>
                    interesses
                        .FirstOrDefault(i => i.CategoriaId == vc.CategoriaId)
                        ?.Pontuacao ?? 0),

            Motivos = v.ViagemCategorias
                .Where(vc =>
                    interesses.Any(i =>
                        i.CategoriaId == vc.CategoriaId))
                .Select(vc => vc.Categoria.Nome)
                .ToList(),

            Categorias = v.ViagemCategorias
                .Select(vc => new
                {
                    vc.Categoria.Id,
                    vc.Categoria.Nome
                }).ToList()
        })
            .Where(v => v.Pontuacao > 0)
            .OrderByDescending(v => v.Pontuacao)
            .ToList();

        return Ok(new
        {
            usuarioId = id,
            recomendacoes
        });

    }
    
   
}
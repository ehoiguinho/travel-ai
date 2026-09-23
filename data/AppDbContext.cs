using Microsoft.EntityFrameworkCore;
using TravelAI.Models;

namespace TravelAI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Viagem> Viagens => Set<Viagem>();

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Compra> Compras => Set<Compra>();
}
using Microsoft.EntityFrameworkCore;
using TravelAI.Models;
using Pgvector;

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
    public DbSet<ViagemCategoria> ViagemCategorias => Set<ViagemCategoria>();
    public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Viagem>(entity =>
        {
            entity.ToTable("viagens");

            entity.HasKey(v => v.Id);

            entity.Property(v => v.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(v => v.Pais)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Descricao)
                .IsRequired();

            entity.Property(v => v.Preco)
                .HasPrecision(10, 2);

            entity.Property(v => v.DuracaoDias)
                .IsRequired();
        });

            modelBuilder.Entity<ViagemCategoria>(entity =>
        {
            entity.ToTable("viagem_categorias");

            entity.HasKey(vc => new
            {
                vc.ViagemId,
                vc.CategoriaId
            });

            entity.HasOne(vc => vc.Viagem)
                .WithMany(v => v.ViagemCategorias)
                .HasForeignKey(vc => vc.ViagemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(vc => vc.Categoria)
                .WithMany(c => c.ViagemCategorias)
                .HasForeignKey(vc => vc.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("compras");

            entity.HasKey(c => c.Id);

            entity.HasOne(c => c.Usuario)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Viagem)
                .WithMany(v => v.Compras)
                .HasForeignKey(c => c.ViagemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KnowledgeChunk>(entity =>
        {
            entity.ToTable("knowledge_chunks");

            entity.HasKey(k => k.Id);

            entity.Property(k => k.Content)
                .IsRequired();

            entity.Property(k => k.Embedding)
                .HasColumnType("vector(1024)");

            entity.HasOne(k => k.Viagem)
                .WithMany()
                .HasForeignKey(k => k.ViagemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
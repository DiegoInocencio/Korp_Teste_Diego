using Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Persistence;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produtos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Saldo).IsRequired();
            entity.Property(e => e.Versao).IsRowVersion();
        });

        modelBuilder.Entity<Movimentacao>(entity =>
        {
            entity.ToTable("Movimentacoes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantidade).IsRequired();
            entity.Property(e => e.Tipo).IsRequired();
            entity.Property(e => e.DataMovimentacao).IsRequired();

            entity.HasOne(m => m.Produto)
                  .WithMany()
                  .HasForeignKey(m => m.ProdutoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Estoque.Infrastructure.Persistence.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<IRepositoryTransaction> IniciarTransacaoAsync()
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        return new RepositoryTransaction(transaction);
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }

    public Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public Task ExcluirAsync(Produto produto)
    {
        _context.Produtos.Remove(produto);
        return Task.CompletedTask;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<bool> PossuiMovimentacoesAsync(Guid produtoId)
    {
        return await _context.Movimentacoes.AnyAsync(m => m.ProdutoId == produtoId);
    }

    public async Task RegistrarMovimentacaoAsync(Movimentacao movimentacao)
    {
        await _context.Movimentacoes.AddAsync(movimentacao);
    }

    public async Task<IEnumerable<Movimentacao>> ObterMovimentacoesPorProdutoIdAsync(Guid produtoId)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .Where(m => m.ProdutoId == produtoId)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Produto> Produtos, int TotalItens)> ListarPaginadoAsync(int pageNumber, int pageSize)
    {
        var query = _context.Produtos.AsNoTracking().OrderBy(p => p.Codigo);
        var totalItens = await query.CountAsync();
        var produtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (produtos, totalItens);
    }

    public async Task CommitarAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Produto>> ListarTodosAsync()
    {
        return await _context.Produtos.AsNoTracking().ToListAsync();
    }

    private sealed class RepositoryTransaction : IRepositoryTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public RepositoryTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync() => _transaction.CommitAsync();

        public Task RollbackAsync() => _transaction.RollbackAsync();

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
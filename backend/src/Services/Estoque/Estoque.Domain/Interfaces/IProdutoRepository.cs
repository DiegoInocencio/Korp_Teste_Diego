using Estoque.Domain.Entities;

namespace Estoque.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<IRepositoryTransaction> IniciarTransacaoAsync();
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task ExcluirAsync(Produto produto);
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<Produto?> ObterPorCodigoAsync(string codigo);
    Task<bool> PossuiMovimentacoesAsync(Guid produtoId);
    Task RegistrarMovimentacaoAsync(Movimentacao movimentacao);
    Task<IEnumerable<Movimentacao>> ObterMovimentacoesPorProdutoIdAsync(Guid produtoId);
    Task<(IEnumerable<Produto> Produtos, int TotalItens)> ListarPaginadoAsync(int pageNumber, int pageSize);
    Task CommitarAsync();
    Task<IEnumerable<Produto>> ListarTodosAsync();
}
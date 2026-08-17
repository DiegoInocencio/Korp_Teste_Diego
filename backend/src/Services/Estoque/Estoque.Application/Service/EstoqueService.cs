using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IProdutoRepository _produtoRepository;

    public EstoqueService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task RegistrarMovimentacaoAsync(RegistrarMovimentacaoDto dto)
    {
        await using var transaction = await _produtoRepository.IniciarTransacaoAsync();

        try
        {
            var produto = await _produtoRepository.ObterPorIdAsync(dto.ProdutoId);

            if (produto == null)
                throw new InvalidOperationException("Produto não encontrado.");

            var tipo = (TipoMovimentacao)dto.Tipo;

            if (tipo == TipoMovimentacao.Entrada)
            {
                produto.CreditarEstoque(dto.Quantidade);
            }
            else
            {
                produto.BaixarEstoque(dto.Quantidade);
            }

            var movimentacao = new Movimentacao(produto.Id, dto.Quantidade, tipo);

            await _produtoRepository.RegistrarMovimentacaoAsync(movimentacao);
            await _produtoRepository.CommitarAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
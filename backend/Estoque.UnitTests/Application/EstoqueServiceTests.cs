using Estoque.Application.DTOs;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Moq;

namespace Estoque.UnitTests.Application;

public class EstoqueServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IRepositoryTransaction> _transactionMock;
    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _transactionMock = new Mock<IRepositoryTransaction>();

        _produtoRepositoryMock
            .Setup(r => r.IniciarTransacaoAsync())
            .ReturnsAsync(_transactionMock.Object);

        _service = new EstoqueService(_produtoRepositoryMock.Object);
    }

    [Fact(DisplayName = "RegistrarMovimentacaoAsync - Deve registrar entrada com sucesso")]
    public async Task RegistrarMovimentacaoAsync_DeveRegistrarEntradaComSucesso()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 100);
        var dto = new RegistrarMovimentacaoDto(produto.Id, 20, 1);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produto.Id))
            .ReturnsAsync(produto);

        await _service.RegistrarMovimentacaoAsync(dto);

        Assert.Equal(120, produto.Saldo);

        _produtoRepositoryMock.Verify(
            r => r.RegistrarMovimentacaoAsync(It.Is<Movimentacao>(m =>
                m.ProdutoId == produto.Id &&
                m.Quantidade == 20 &&
                m.Tipo == TipoMovimentacao.Entrada)),
            Times.Once);

        _produtoRepositoryMock.Verify(r => r.CommitarAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(), Times.Never);
    }

    [Fact(DisplayName = "RegistrarMovimentacaoAsync - Deve registrar saída com sucesso")]
    public async Task RegistrarMovimentacaoAsync_DeveRegistrarSaidaComSucesso()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 100);
        var dto = new RegistrarMovimentacaoDto(produto.Id, 30, 2);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produto.Id))
            .ReturnsAsync(produto);

        await _service.RegistrarMovimentacaoAsync(dto);

        Assert.Equal(70, produto.Saldo);

        _produtoRepositoryMock.Verify(
            r => r.RegistrarMovimentacaoAsync(It.Is<Movimentacao>(m =>
                m.ProdutoId == produto.Id &&
                m.Quantidade == 30 &&
                m.Tipo == TipoMovimentacao.Saida)),
            Times.Once);

        _produtoRepositoryMock.Verify(r => r.CommitarAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(), Times.Once);
        _transactionMock.Verify(t => t.RollbackAsync(), Times.Never);
    }

    [Fact(DisplayName = "RegistrarMovimentacaoAsync - Deve realizar rollback quando saída tiver saldo insuficiente")]
    public async Task RegistrarMovimentacaoAsync_DeveRealizarRollback_QuandoSaldoInsuficiente()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 10);
        var dto = new RegistrarMovimentacaoDto(produto.Id, 50, 2);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produto.Id))
            .ReturnsAsync(produto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarMovimentacaoAsync(dto));

        _transactionMock.Verify(t => t.RollbackAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(), Times.Never);
        _produtoRepositoryMock.Verify(r => r.RegistrarMovimentacaoAsync(It.IsAny<Movimentacao>()), Times.Never);
        _produtoRepositoryMock.Verify(r => r.CommitarAsync(), Times.Never);
    }

    [Fact(DisplayName = "RegistrarMovimentacaoAsync - Deve falhar e realizar rollback quando produto não encontrado")]
    public async Task RegistrarMovimentacaoAsync_DeveFalharERollback_QuandoProdutoNaoEncontrado()
    {
        var produtoId = Guid.NewGuid();
        var dto = new RegistrarMovimentacaoDto(produtoId, 10, 1);

        _produtoRepositoryMock
            .Setup(r => r.ObterPorIdAsync(produtoId))
            .ReturnsAsync((Produto?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarMovimentacaoAsync(dto));

        Assert.Equal("Produto não encontrado.", exception.Message);
        _transactionMock.Verify(t => t.RollbackAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(), Times.Never);
        _produtoRepositoryMock.Verify(r => r.RegistrarMovimentacaoAsync(It.IsAny<Movimentacao>()), Times.Never);
        _produtoRepositoryMock.Verify(r => r.CommitarAsync(), Times.Never);
    }
}
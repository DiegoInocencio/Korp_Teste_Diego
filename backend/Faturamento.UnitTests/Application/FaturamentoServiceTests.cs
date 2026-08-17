using Faturamento.Application.DTOs;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;
using Faturamento.Application.Services;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;
using Moq;

namespace Faturamento.UnitTests.Application;

public class FaturamentoServiceTests
{
    private readonly Mock<INotaFiscalRepository> _notaFiscalRepositoryMock;
    private readonly Mock<IEstoqueIntegrationService> _estoqueIntegrationServiceMock;
    private readonly FaturamentoService _service;

    public FaturamentoServiceTests()
    {
        _notaFiscalRepositoryMock = new Mock<INotaFiscalRepository>();
        _estoqueIntegrationServiceMock = new Mock<IEstoqueIntegrationService>();

        _service = new FaturamentoService(
            _notaFiscalRepositoryMock.Object,
            _estoqueIntegrationServiceMock.Object);
    }

    [Fact(DisplayName = "ImprimirNotaAsync - Deve fechar nota quando integração com estoque for bem-sucedida")]
    public async Task ImprimirNotaAsync_DeveFecharNota_QuandoIntegracaoComEstoqueForBemSucedida()
    {
        var nota = CriarNotaAbertaComItem();

        _notaFiscalRepositoryMock
            .Setup(r => r.ObterPorIdAsync(nota.Id))
            .ReturnsAsync(nota);

        await _service.ImprimirNotaAsync(nota.Id);

        Assert.Equal(StatusNotaFiscal.Fechada, nota.Status);

        _estoqueIntegrationServiceMock.Verify(
            s => s.BaixarEstoqueAsync(It.IsAny<IEnumerable<BaixaEstoqueItemDto>>()),
            Times.Once);

        _notaFiscalRepositoryMock.Verify(r => r.AtualizarAsync(nota), Times.Once);
    }

    [Fact(DisplayName = "ImprimirNotaAsync - Deve manter nota aberta quando integração com estoque falhar")]
    public async Task ImprimirNotaAsync_DeveManterNotaAberta_QuandoIntegracaoComEstoqueFalhar()
    {
        var nota = CriarNotaAbertaComItem();

        _notaFiscalRepositoryMock
            .Setup(r => r.ObterPorIdAsync(nota.Id))
            .ReturnsAsync(nota);

        _estoqueIntegrationServiceMock
            .Setup(s => s.BaixarEstoqueAsync(It.IsAny<IEnumerable<BaixaEstoqueItemDto>>()))
            .ThrowsAsync(new EstoqueIntegrationException("Não foi possível comunicar com o Estoque."));

        var ex = await Assert.ThrowsAsync<EstoqueIntegrationException>(() => _service.ImprimirNotaAsync(nota.Id));

        Assert.Equal("Não foi possível comunicar com o Estoque.", ex.Message);
        Assert.Equal(StatusNotaFiscal.Aberta, nota.Status);

        _notaFiscalRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<NotaFiscal>()), Times.Never);
    }

    private static NotaFiscal CriarNotaAbertaComItem()
    {
        var nota = new NotaFiscal(Guid.NewGuid());
        nota.AdicionarItem(Guid.NewGuid(), 2);
        return nota;
    }
}

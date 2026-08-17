using Faturamento.Application.DTOs;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;

namespace Faturamento.Application.Services;

public class FaturamentoService : IFaturamentoService
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IEstoqueIntegrationService _estoqueIntegrationService;

    public FaturamentoService(
        INotaFiscalRepository notaFiscalRepository,
        IEstoqueIntegrationService estoqueIntegrationService)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _estoqueIntegrationService = estoqueIntegrationService;
    }

    public async Task<CriarNotaFiscalResponseDto> CriarNotaAsync(CriarNotaFiscalRequestDto dto)
    {
        var notaFiscal = new NotaFiscal(Guid.NewGuid());
        notaFiscal.AdicionarItem(dto.ProdutoId, dto.Quantidade);

        await _notaFiscalRepository.CriarAsync(notaFiscal);

        return new CriarNotaFiscalResponseDto(notaFiscal.Id, notaFiscal.Numero, (int)notaFiscal.Status);
    }

    public async Task AdicionarItemAsync(Guid notaFiscalId, AdicionarNotaFiscalItemRequestDto dto)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(notaFiscalId)
            ?? throw new KeyNotFoundException("Nota fiscal não encontrada.");

        notaFiscal.AdicionarItem(dto.ProdutoId, dto.Quantidade);

        await _notaFiscalRepository.AtualizarAsync(notaFiscal);
    }

    public async Task<NotaFiscalDto?> ObterPorIdAsync(Guid id)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(id);
        return notaFiscal is null ? null : MapearNotaFiscal(notaFiscal);
    }

    public async Task ImprimirNotaAsync(Guid notaId)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(notaId)
            ?? throw new KeyNotFoundException("Nota fiscal não encontrada.");

        if (notaFiscal.Status != StatusNotaFiscal.Aberta)
            throw new InvalidOperationException("A nota fiscal deve estar aberta para impressão.");

        if (notaFiscal.Itens.Count == 0)
            throw new InvalidOperationException("A nota fiscal deve possuir ao menos um item para impressão.");

        var itensParaBaixa = notaFiscal.Itens
            .Select(i => new BaixaEstoqueItemDto(i.ProdutoId, i.Quantidade));

        try
        {
            await _estoqueIntegrationService.BaixarEstoqueAsync(itensParaBaixa);
        }
        catch (EstoqueIntegrationException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new EstoqueIntegrationException("Não foi possível comunicar com o Estoque.");
        }

        notaFiscal.Fechar();
        await _notaFiscalRepository.AtualizarAsync(notaFiscal);
    }

    private static NotaFiscalDto MapearNotaFiscal(NotaFiscal notaFiscal)
    {
        var itens = notaFiscal.Itens
            .Select(i => new NotaFiscalItemDto(i.Id, i.ProdutoId, i.Quantidade))
            .ToArray();

        return new NotaFiscalDto(notaFiscal.Id, notaFiscal.Numero, (int)notaFiscal.Status, itens);
    }
}

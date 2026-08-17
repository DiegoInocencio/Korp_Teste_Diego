using Faturamento.Application.DTOs;

namespace Faturamento.Application.Interfaces;

public interface IFaturamentoService
{
    Task<CriarNotaFiscalResponseDto> CriarNotaAsync(CriarNotaFiscalRequestDto dto);
    Task AdicionarItemAsync(Guid notaFiscalId, AdicionarNotaFiscalItemRequestDto dto);
    Task ImprimirNotaAsync(Guid notaId);
    Task<NotaFiscalDto?> ObterPorIdAsync(Guid id);
}

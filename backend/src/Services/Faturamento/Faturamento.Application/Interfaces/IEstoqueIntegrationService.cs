using Faturamento.Application.DTOs;

namespace Faturamento.Application.Interfaces;

public interface IEstoqueIntegrationService
{
    Task BaixarEstoqueAsync(IEnumerable<BaixaEstoqueItemDto> itens);
}

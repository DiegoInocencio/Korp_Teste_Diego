using Faturamento.Domain.Entities;

namespace Faturamento.Domain.Interfaces;

public interface INotaFiscalRepository
{
    Task CriarAsync(NotaFiscal notaFiscal);
    Task<NotaFiscal?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(NotaFiscal notaFiscal);
}

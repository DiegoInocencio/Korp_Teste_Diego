using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Persistence.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(NotaFiscal notaFiscal)
    {
        await _context.NotasFiscais.AddAsync(notaFiscal);
        await _context.SaveChangesAsync();
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task AtualizarAsync(NotaFiscal notaFiscal)
    {
        await _context.SaveChangesAsync();
    }
}

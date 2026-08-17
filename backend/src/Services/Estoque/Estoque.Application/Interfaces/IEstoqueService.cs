using Estoque.Application.DTOs;

namespace Estoque.Application.Interfaces;

public interface IEstoqueService
{
    Task RegistrarMovimentacaoAsync(RegistrarMovimentacaoDto dto);
}

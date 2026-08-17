namespace Estoque.Application.DTOs;

public record RegistrarMovimentacaoDto(
    Guid ProdutoId,
    int Quantidade,
    int Tipo
);
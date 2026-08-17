namespace Faturamento.Application.DTOs;

public record CriarNotaFiscalRequestDto(Guid ProdutoId, int Quantidade);

public record CriarNotaFiscalResponseDto(Guid Id, int Numero, int Status);

public record AdicionarNotaFiscalItemRequestDto(Guid ProdutoId, int Quantidade);

public record NotaFiscalItemDto(Guid Id, Guid ProdutoId, int Quantidade);

public record NotaFiscalDto(Guid Id, int Numero, int Status, IReadOnlyCollection<NotaFiscalItemDto> Itens);

public record BaixaEstoqueItemDto(Guid ProdutoId, int Quantidade);

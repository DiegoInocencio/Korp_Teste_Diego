namespace Estoque.Application.DTOs;

public record CadastrarProdutoRequest(
    string Codigo,
    string Descricao,
    int SaldoInicial
);

public record AtualizarDescricaoProdutoRequest(string Descricao);

public record ProdutoResponseDto(
    Guid Id,
    string Codigo,
    string Descricao,
    int Saldo
);

public record PaginacaoDto<T>(
    IEnumerable<T> Itens,
    int PageNumber,
    int PageSize,
    int TotalItens
);

public record MovimentacaoResponseDto(
    Guid Id,
    Guid ProdutoId,
    int Quantidade,
    int Tipo,
    DateTime DataMovimentacao
);

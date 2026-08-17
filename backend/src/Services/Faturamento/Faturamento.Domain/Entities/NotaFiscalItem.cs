namespace Faturamento.Domain.Entities;

public class NotaFiscalItem
{
    public Guid Id { get; private set; }
    public Guid NotaFiscalId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }

    public NotaFiscal NotaFiscal { get; private set; } = null!;

    protected NotaFiscalItem() { }

    public NotaFiscalItem(Guid produtoId, int quantidade)
    {
        if (produtoId == Guid.Empty)
            throw new ArgumentException("ProdutoId é obrigatório.");

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        Quantidade = quantidade;
    }
}

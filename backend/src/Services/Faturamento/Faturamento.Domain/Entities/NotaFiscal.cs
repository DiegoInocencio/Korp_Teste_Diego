namespace Faturamento.Domain.Entities;

public class NotaFiscal
{
    private readonly List<NotaFiscalItem> _itens = [];

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; }
    public IReadOnlyCollection<NotaFiscalItem> Itens => _itens.AsReadOnly();

    protected NotaFiscal() { }

    public NotaFiscal(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id da nota fiscal é obrigatório.");

        Id = id;
        Status = StatusNotaFiscal.Aberta;
    }

    public void AdicionarItem(Guid produtoId, int quantidade)
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new InvalidOperationException("Não é possível adicionar itens em uma nota fiscal fechada.");

        var item = new NotaFiscalItem(produtoId, quantidade);
        _itens.Add(item);
    }

    public void Fechar()
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new InvalidOperationException("A nota fiscal já está fechada.");

        if (_itens.Count == 0)
            throw new InvalidOperationException("A nota fiscal deve possuir ao menos um item para impressão.");

        Status = StatusNotaFiscal.Fechada;
    }
}

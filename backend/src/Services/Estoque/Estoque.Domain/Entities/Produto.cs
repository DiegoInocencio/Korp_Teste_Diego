namespace Estoque.Domain.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int Saldo { get; private set; }
    public uint Versao { get; private set; }

    protected Produto() { }


    public Produto(string codigo, string descricao, int saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("O código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição do produto é obrigatória.");

        if (saldoInicial < 0)
            throw new ArgumentException("O saldo inicial não pode ser negativo.");

        Id = Guid.NewGuid();
        Codigo = codigo;
        Descricao = descricao;
        Saldo = saldoInicial;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a ser baixada deve ser maior que zero.");

        if (Saldo < quantidade)
            throw new InvalidOperationException($"Saldo insuficiente. Saldo atual: {Saldo}, Tentativa de baixa: {quantidade}");

        Saldo -= quantidade;
    }

    public void CreditarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade a ser creditada deve ser maior que zero.");

        Saldo += quantidade;
    }

    public void AtualizarDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição do produto é obrigatória.");

        Descricao = descricao;
    }
}
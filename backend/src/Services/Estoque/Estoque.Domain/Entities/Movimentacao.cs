using System;

namespace Estoque.Domain.Entities
{
    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2
    }

    public class Movimentacao
    {
        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public DateTime DataMovimentacao { get; private set; }
        public TipoMovimentacao Tipo { get; private set; }

        public virtual Produto Produto { get; private set; } = null!;

        protected Movimentacao() { }

        public Movimentacao(Guid produtoId, int quantidade, TipoMovimentacao tipo)
        {
            if (produtoId == Guid.Empty)
                throw new ArgumentException("O ID do produto é obrigatório.");

            if (quantidade <= 0)
                throw new ArgumentException("A quantidade da movimentação deve ser maior que zero.");

            Id = Guid.NewGuid();
            ProdutoId = produtoId;
            Quantidade = quantidade;
            Tipo = tipo;
            DataMovimentacao = DateTime.UtcNow;
        }
    }
}
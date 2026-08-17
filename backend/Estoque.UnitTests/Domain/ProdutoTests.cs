using Estoque.Domain.Entities;
using System;
using Xunit;

namespace Estoque.UnitTests.Domain;

public class ProdutoTests
{
    [Fact(DisplayName = "Baixar Estoque - Deve diminuir o saldo quando a quantidade for válida")]
    public void BaixarEstoque_DeveDiminuirSaldo_QuandoQuantidadeValida()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 100);

        produto.BaixarEstoque(20);

        Assert.Equal(80, produto.Saldo);
    }

    [Fact(DisplayName = "Baixar Estoque - Deve lançar exceção quando o saldo for insuficiente")]
    public void BaixarEstoque_DeveLancarExcecao_QuandoSaldoInsuficiente()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 30);

        var excecao = Assert.Throws<InvalidOperationException>(() => produto.BaixarEstoque(50));

        Assert.Contains("Saldo insuficiente", excecao.Message);
    }

    [Fact(DisplayName = "Creditar Estoque - Deve somar ao saldo existente")]
    public void CreditarEstoque_DeveSomarAoSaldo_QuandoQuantidadeValida()
    {
        var produto = new Produto("CIM-50", "Cimento 50kg", 100);

        produto.CreditarEstoque(50);

        Assert.Equal(150, produto.Saldo);
    }
}
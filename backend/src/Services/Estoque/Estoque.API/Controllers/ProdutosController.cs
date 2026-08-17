using Estoque.Application.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{   
    private readonly IProdutoRepository _repository;

    public ProdutosController(IProdutoRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarProduto([FromBody] CadastrarProdutoRequest request)
    {
        var produtoExistente = await _repository.ObterPorCodigoAsync(request.Codigo);
        if (produtoExistente != null)
        {
            return BadRequest(new { Mensagem = "Um produto com este código já está cadastrado." });
        }

        var produto = new Produto(request.Codigo, request.Descricao, request.SaldoInicial);

        await _repository.AdicionarAsync(produto);
        await _repository.CommitarAsync();

        return CreatedAtAction(nameof(CadastrarProduto), new { id = produto.Id }, MapearProduto(produto));
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { Mensagem = "pageNumber e pageSize devem ser maiores que zero." });
        }

        var (produtos, totalItens) = await _repository.ListarPaginadoAsync(pageNumber, pageSize);
        var resposta = new PaginacaoDto<ProdutoResponseDto>(
            produtos.Select(MapearProduto),
            pageNumber,
            pageSize,
            totalItens);

        return Ok(resposta);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarDescricao(Guid id, [FromBody] AtualizarDescricaoProdutoRequest request)
    {
        var produto = await _repository.ObterPorIdAsync(id);
        if (produto is null)
        {
            return NotFound(new { Mensagem = "Produto não encontrado." });
        }

        produto.AtualizarDescricao(request.Descricao);

        await _repository.AtualizarAsync(produto);
        await _repository.CommitarAsync();

        return Ok(MapearProduto(produto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        var produto = await _repository.ObterPorIdAsync(id);
        if (produto is null)
        {
            return NotFound(new { Mensagem = "Produto não encontrado." });
        }

        var possuiMovimentacoes = await _repository.PossuiMovimentacoesAsync(id);
        if (possuiMovimentacoes)
        {
            return BadRequest(new { Mensagem = "Não é possível excluir produto com movimentações." });
        }

        await _repository.ExcluirAsync(produto);
        await _repository.CommitarAsync();

        return NoContent();
    }

    private static ProdutoResponseDto MapearProduto(Produto produto)
        => new(produto.Id, produto.Codigo, produto.Descricao, produto.Saldo);
}
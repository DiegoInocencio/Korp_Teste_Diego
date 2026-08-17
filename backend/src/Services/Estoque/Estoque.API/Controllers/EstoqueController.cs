using Estoque.Application.DTOs;
using Estoque.Application.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;
    private readonly IProdutoRepository _produtoRepository;

    public EstoqueController(IEstoqueService estoqueService, IProdutoRepository produtoRepository)
    {
        _estoqueService = estoqueService;
        _produtoRepository = produtoRepository;
    }

    [HttpPost("movimentar")]
    public async Task<IActionResult> RegistrarMovimentacao([FromBody] RegistrarMovimentacaoDto dto)
    {
        await _estoqueService.RegistrarMovimentacaoAsync(dto);
        return Ok(new { message = "Movimentação registrada com sucesso!" });
    }

    [HttpGet("{produtoId:guid}/movimentacoes")]
    public async Task<IActionResult> ObterMovimentacoes(Guid produtoId)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
        if (produto is null)
        {
            return NotFound(new { Mensagem = "Produto não encontrado." });
        }

        var movimentacoes = await _produtoRepository.ObterMovimentacoesPorProdutoIdAsync(produtoId);
        var resposta = movimentacoes.Select(MapearMovimentacao);
        return Ok(resposta);
    }

    private static MovimentacaoResponseDto MapearMovimentacao(Movimentacao movimentacao)
        => new(
            movimentacao.Id,
            movimentacao.ProdutoId,
            movimentacao.Quantidade,
            (int)movimentacao.Tipo,
            movimentacao.DataMovimentacao
        );
}
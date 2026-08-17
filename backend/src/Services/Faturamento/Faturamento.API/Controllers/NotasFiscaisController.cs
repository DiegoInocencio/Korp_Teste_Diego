using Faturamento.Application.DTOs;
using Faturamento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasFiscaisController : ControllerBase
{
    private readonly IFaturamentoService _faturamentoService;

    public NotasFiscaisController(IFaturamentoService faturamentoService)
    {
        _faturamentoService = faturamentoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarNota([FromBody] CriarNotaFiscalRequestDto request)
    {
        var nota = await _faturamentoService.CriarNotaAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = nota.Id }, nota);
    }

    [HttpPost("{id:guid}/itens")]
    public async Task<IActionResult> AdicionarItem(Guid id, [FromBody] AdicionarNotaFiscalItemRequestDto request)
    {
        await _faturamentoService.AdicionarItemAsync(id, request);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var nota = await _faturamentoService.ObterPorIdAsync(id);
        if (nota is null)
            return NotFound(new { mensagem = "Nota fiscal não encontrada." });

        return Ok(nota);
    }

    [HttpPost("{id:guid}/imprimir")]
    public async Task<IActionResult> Imprimir(Guid id)
    {
        await _faturamentoService.ImprimirNotaAsync(id);
        return Ok(new { mensagem = "Nota fiscal impressa com sucesso." });
    }
}

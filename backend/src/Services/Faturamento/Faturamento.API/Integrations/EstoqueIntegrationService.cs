using Faturamento.Application.DTOs;
using Faturamento.Application.Exceptions;
using Faturamento.Application.Interfaces;

namespace Faturamento.API.Integrations;

public class EstoqueIntegrationService : IEstoqueIntegrationService
{
    private readonly HttpClient _httpClient;

    public EstoqueIntegrationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task BaixarEstoqueAsync(IEnumerable<BaixaEstoqueItemDto> itens)
    {
        foreach (var item in itens)
        {
            var request = new
            {
                produtoId = item.ProdutoId,
                quantidade = item.Quantidade,
                tipo = 2
            };

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsJsonAsync("api/Estoque/movimentar", request);
            }
            catch (HttpRequestException)
            {
                throw new EstoqueIntegrationException("Não foi possível comunicar com o Estoque.");
            }
            catch (TaskCanceledException)
            {
                throw new EstoqueIntegrationException("Não foi possível comunicar com o Estoque.");
            }

            if (!response.IsSuccessStatusCode)
                throw new EstoqueIntegrationException("Não foi possível comunicar com o Estoque.");
        }
    }
}

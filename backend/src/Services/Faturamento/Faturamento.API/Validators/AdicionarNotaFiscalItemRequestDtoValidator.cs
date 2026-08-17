using Faturamento.Application.DTOs;
using FluentValidation;

namespace Faturamento.API.Validators;

public class AdicionarNotaFiscalItemRequestDtoValidator : AbstractValidator<AdicionarNotaFiscalItemRequestDto>
{
    public AdicionarNotaFiscalItemRequestDtoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .NotEmpty()
            .WithMessage("ProdutoId é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que zero.");
    }
}

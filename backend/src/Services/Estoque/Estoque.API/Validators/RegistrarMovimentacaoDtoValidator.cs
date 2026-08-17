using Estoque.Application.DTOs;
using FluentValidation;

namespace Estoque.API.Validators;

public class RegistrarMovimentacaoDtoValidator : AbstractValidator<RegistrarMovimentacaoDto>
{
    public RegistrarMovimentacaoDtoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .NotEmpty()
            .WithMessage("ProdutoId é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("Quantidade deve ser maior que zero.");

        RuleFor(x => x.Tipo)
            .Must(tipo => tipo is 1 or 2)
            .WithMessage("Tipo deve ser 1 (Entrada) ou 2 (Saída).");
    }
}

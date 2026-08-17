using Estoque.Application.DTOs;
using FluentValidation;

namespace Estoque.API.Validators;

public class CadastrarProdutoRequestValidator : AbstractValidator<CadastrarProdutoRequest>
{
    public CadastrarProdutoRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("Código é obrigatório.");

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória.");

        RuleFor(x => x.SaldoInicial)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Saldo inicial deve ser maior ou igual a zero.");
    }
}

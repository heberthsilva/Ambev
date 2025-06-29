using FluentValidation;
using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    public class DeleteSaleRequestValidator : AbstractValidator<DeleteSaleRequest>
    {
        public DeleteSaleRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O ID da venda é obrigatório para exclusão.");
        }
    }
}
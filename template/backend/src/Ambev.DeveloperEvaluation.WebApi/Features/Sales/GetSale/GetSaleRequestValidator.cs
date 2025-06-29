using FluentValidation;
using System;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleRequestValidator : AbstractValidator<GetSaleRequest>
    {
        public GetSaleRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O ID da venda é obrigatório.");
        }
    }
}
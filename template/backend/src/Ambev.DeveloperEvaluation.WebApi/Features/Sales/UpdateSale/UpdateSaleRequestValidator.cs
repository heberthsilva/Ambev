using FluentValidation;
using System.Linq;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
    {
        public UpdateSaleRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("O ID da venda é obrigatório para atualização.");
            RuleFor(x => x.SaleNumber).NotEmpty().WithMessage("O número da venda é obrigatório.");
            RuleFor(x => x.SaleDate).NotEmpty().WithMessage("A data da venda é obrigatória.");
            RuleFor(x => x.Customer).NotEmpty().WithMessage("O cliente é obrigatório.").MaximumLength(255);
            RuleFor(x => x.Branch).NotEmpty().WithMessage("A filial é obrigatória.").MaximumLength(100);
            RuleFor(x => x.Items).NotEmpty().WithMessage("A venda deve conter pelo menos um item.");
            RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemRequestValidator());
        }
    }

    public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
    {
        public UpdateSaleItemRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("O ID do produto é obrigatório.");
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("O nome do produto é obrigatório.").MaximumLength(255);
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("A quantidade é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
                .LessThanOrEqualTo(20).WithMessage("Não é possível vender mais de 20 itens idênticos.");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithMessage("O preço unitário é obrigatório.")
                .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");
        }
    }
}
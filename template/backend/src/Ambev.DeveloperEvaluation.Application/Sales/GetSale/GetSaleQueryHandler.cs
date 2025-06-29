using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleQueryHandler : IRequestHandler<GetSaleQuery, GetSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;

        public GetSaleQueryHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<GetSaleResponse> Handle(GetSaleQuery request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
            {
                return null; // Or throw a NotFoundException
            }

            return new GetSaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                Customer = sale.Customer,
                TotalAmount = sale.TotalAmount,
                Branch = sale.Branch,
                IsCancelled = sale.IsCancelled,
                Items = sale.Items.Select(item => new GetSaleItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalItemAmount = item.TotalItemAmount,
                    IsCancelled = item.IsCancelled
                }).ToList()
            };
        }
    }
}
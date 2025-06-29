using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateSaleCommandHandler> _logger;

        public CreateSaleCommandHandler(ISaleRepository saleRepository, IMediator mediator, ILogger<CreateSaleCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<CreateSaleResponse> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale(request.SaleNumber, request.SaleDate, request.Customer, request.Branch);

            foreach (var itemCommand in request.Items)
            {
                var saleItem = new SaleItem(itemCommand.ProductId, itemCommand.ProductName, itemCommand.Quantity, itemCommand.UnitPrice);
                sale.AddItem(saleItem);
            }

            await _saleRepository.CreateAsync(sale, cancellationToken);

            foreach (var domainEvent in sale.DomainEvents)
            {
                _logger.LogInformation("Publicando evento de domínio: {EventType}", domainEvent.GetType().Name);
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            sale.ClearDomainEvents();

            return new CreateSaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                Customer = sale.Customer,
                TotalAmount = sale.TotalAmount,
                Branch = sale.Branch,
                IsCancelled = sale.IsCancelled
            };
        }
    }
}
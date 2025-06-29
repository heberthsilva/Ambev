using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<DeleteSaleCommandHandler> _logger;

        public DeleteSaleCommandHandler(ISaleRepository saleRepository, IMediator mediator, ILogger<DeleteSaleCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
            if (sale == null)
            {
                return new DeleteSaleResponse { Success = false };
            }

            var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);

            if (success)
            {
                foreach (var domainEvent in sale.DomainEvents)
                {
                    _logger.LogInformation("Publicando evento de domínio: {EventType}", domainEvent.GetType().Name);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
                sale.ClearDomainEvents();
            }

            return new DeleteSaleResponse { Success = success };
        }
    }
}
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateSaleCommandHandler> _logger;

        public UpdateSaleCommandHandler(ISaleRepository saleRepository, IMediator mediator, ILogger<UpdateSaleCommandHandler> logger)
        {
            _saleRepository = saleRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<UpdateSaleResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
            {
                return null; // Or throw a NotFoundException
            }

            sale.UpdateSaleDetails(request.SaleNumber, request.SaleDate, request.Customer, request.Branch);

            if (request.IsCancelled && !sale.IsCancelled)
            {
                sale.CancelSale();
            }

            // Process items: update existing, add new, and mark for removal
            var existingItemIds = sale.Items.Select(i => i.Id).ToHashSet();
            var requestItemIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id.Value).ToHashSet();

            // Remove items not in the request
            var itemsToRemove = sale.Items.Where(item => !requestItemIds.Contains(item.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                sale.Items.Remove(item);
            }

            foreach (var itemCommand in request.Items)
            {
                if (itemCommand.Id.HasValue && existingItemIds.Contains(itemCommand.Id.Value))
                {
                    // Update existing item
                    var existingItem = sale.Items.FirstOrDefault(i => i.Id == itemCommand.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.SetQuantity(itemCommand.Quantity);
                        if (itemCommand.IsCancelled && !existingItem.IsCancelled)
                        {
                            existingItem.CancelItem(sale.Id);
                        }
                        else if (!itemCommand.IsCancelled && existingItem.IsCancelled)
                        {
                            // Reativar item se necessário, ou lançar exceção se não permitido
                            // Por simplicidade, não há método para reativar, então apenas não cancela novamente
                        }
                    }
                }
                else
                {
                    // Add new item
                    var newItem = new SaleItem(itemCommand.ProductId, itemCommand.ProductName, itemCommand.Quantity, itemCommand.UnitPrice);
                    sale.AddItem(newItem);
                }
            }

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            foreach (var domainEvent in sale.DomainEvents)
            {
                _logger.LogInformation("Publicando evento de domínio: {EventType}", domainEvent.GetType().Name);
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            sale.ClearDomainEvents();

            return new UpdateSaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                Customer = sale.Customer,
                TotalAmount = sale.TotalAmount,
                Branch = sale.Branch,
                IsCancelled = sale.IsCancelled,
                Items = sale.Items.Select(item => new UpdateSaleItemResponse
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
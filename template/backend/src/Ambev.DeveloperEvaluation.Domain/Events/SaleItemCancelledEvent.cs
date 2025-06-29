using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleItemCancelledEvent
    {
        public SaleItem SaleItem { get; }
        public Guid SaleId { get; }

        public SaleItemCancelledEvent(SaleItem saleItem, Guid saleId)
        {
            SaleItem = saleItem;
            SaleId = saleId;
        }
    }
}
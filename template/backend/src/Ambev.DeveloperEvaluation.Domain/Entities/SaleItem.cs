using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Events;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalItemAmount { get; private set; }
        public bool IsCancelled { get; private set; }

        private const int MaxQuantity = 20;
        private const int Discount10PercentThreshold = 4;
        private const int Discount20PercentThreshold = 10;

        public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            IsCancelled = false;
            SetQuantity(quantity);
        }

        public void SetQuantity(int quantity)
        {
            if (quantity > MaxQuantity)
            {
                throw new DomainException($"Não é possível vender mais de {MaxQuantity} itens idênticos.");
            }

            Quantity = quantity;
            CalculateDiscount();
            CalculateTotalItemAmount();
        }

        public void CancelItem(Guid saleId)
        {
            if (!IsCancelled)
            {
                IsCancelled = true;
                TotalItemAmount = 0;
                Discount = 0;
                AddDomainEvent(new SaleItemCancelledEvent(this, saleId));
            }
        }

        private void CalculateDiscount()
        {
            Discount = 0;
            if (Quantity >= Discount20PercentThreshold && Quantity <= MaxQuantity)
            {
                Discount = UnitPrice * Quantity * 0.20m;
            }
            else if (Quantity >= Discount10PercentThreshold && Quantity < Discount20PercentThreshold)
            {
                Discount = UnitPrice * Quantity * 0.10m;
            }
            else
            {
                Discount = 0;
            }
        }

        private void CalculateTotalItemAmount()
        {
            TotalItemAmount = (UnitPrice * Quantity) - Discount;
        }
    }
}
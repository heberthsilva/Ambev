using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public int SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public string Customer { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Branch { get; private set; }
        public bool IsCancelled { get; private set; }
        public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();

        public void UpdateSaleDetails(int saleNumber, DateTime saleDate, string customer, string branch)
        {
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            Customer = customer;
            Branch = branch;
            CalculateTotalAmount();
            AddDomainEvent(new SaleModifiedEvent(this));
        }

        public Sale(int saleNumber, DateTime saleDate, string customer, string branch)
        {
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            Customer = customer;
            Branch = branch;
            IsCancelled = false;
            CalculateTotalAmount();
        }

        public void AddItem(SaleItem item)
        {
            Items.Add(item);
            CalculateTotalAmount();
        }

        public void CancelSale()
        {
            if (!IsCancelled)
            {
                IsCancelled = true;
                foreach (var item in Items)
                {
                    item.CancelItem(Id);
                }
                AddDomainEvent(new SaleCancelledEvent(this));
            }
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = 0;
            foreach (var item in Items)
            {
                TotalAmount += item.TotalItemAmount;
            }
        }
    }
}
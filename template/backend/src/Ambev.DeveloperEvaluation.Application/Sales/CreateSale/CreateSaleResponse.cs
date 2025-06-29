using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleResponse
    {
        public Guid Id { get; set; }
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public string Branch { get; set; }
        public bool IsCancelled { get; set; }
    }
}
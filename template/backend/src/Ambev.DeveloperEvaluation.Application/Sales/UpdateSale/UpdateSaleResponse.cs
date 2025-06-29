using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleResponse
    {
        public Guid Id { get; set; }
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public string Branch { get; set; }
        public bool IsCancelled { get; set; }
        public List<UpdateSaleItemResponse> Items { get; set; }
    }

    public class UpdateSaleItemResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalItemAmount { get; set; }
        public bool IsCancelled { get; set; }
    }
}
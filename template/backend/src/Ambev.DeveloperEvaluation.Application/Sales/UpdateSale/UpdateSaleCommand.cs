using MediatR;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleCommand : IRequest<UpdateSaleResponse>
    {
        public Guid Id { get; set; }
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public bool IsCancelled { get; set; }
        public List<UpdateSaleItemCommand> Items { get; set; }
    }

    public class UpdateSaleItemCommand
    {
        public Guid? Id { get; set; } // Nullable for new items
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsCancelled { get; set; }
    }
}
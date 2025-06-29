using MediatR;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleCommand : IRequest<CreateSaleResponse>
    {
        public int SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string Customer { get; set; }
        public string Branch { get; set; }
        public List<CreateSaleItemCommand> Items { get; set; }
    }

    public class CreateSaleItemCommand
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleModifiedEvent
    {
        public Sale Sale { get; }

        public SaleModifiedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
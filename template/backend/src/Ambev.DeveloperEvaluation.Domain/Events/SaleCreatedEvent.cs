using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCreatedEvent
    {
        public Sale Sale { get; }

        public SaleCreatedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
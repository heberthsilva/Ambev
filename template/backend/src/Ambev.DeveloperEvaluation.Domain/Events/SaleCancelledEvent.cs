using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCancelledEvent
    {
        public Sale Sale { get; }

        public SaleCancelledEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
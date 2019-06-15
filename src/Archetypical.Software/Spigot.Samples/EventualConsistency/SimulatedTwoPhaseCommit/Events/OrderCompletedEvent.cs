using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using System;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events
{
    public class OrderCompletedEvent
    {
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
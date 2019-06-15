using System;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events
{
    public class OrderPlacedEvent
    {
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
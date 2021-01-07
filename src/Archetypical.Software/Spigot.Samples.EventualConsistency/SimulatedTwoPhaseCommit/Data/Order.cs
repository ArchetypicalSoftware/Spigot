using System;
using System.Collections.Generic;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data
{
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public Guid CustomerId { get; set; }

        public Decimal Amount { get; set; }
        public List<string> Items { get; set; }
        public OrderStatus Status { get; internal set; }
    }
}
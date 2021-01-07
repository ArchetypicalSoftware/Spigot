using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using System;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events
{
    public class CreditResultEvent
    {
        public CreditStatus CreditStatus { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }
    }
}
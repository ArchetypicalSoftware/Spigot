using System;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data
{
    public class CustomerCredit
    {
        public Guid OrderReference { get; set; }
        public decimal Amount { get; set; }
        public CreditStatus CreditStatus { get; set; }
    }
}
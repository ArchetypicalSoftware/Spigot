using System;
using System.Collections.Generic;
using System.Linq;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data
{
    public class Customer
    {
        public Customer()
        {
            CustomerId = Guid.NewGuid();
        }

        public decimal CreditLimit { get; set; }

        public decimal AvailableLimit
        {
            get
            {
                return CreditLimit - CustomerCredits.Where(x => x.CreditStatus != CreditStatus.Declined)
                           .Sum(s => s.Amount);
            }
        }

        public Guid CustomerId { get; private set; }
        public List<CustomerCredit> CustomerCredits { get; set; } = new List<CustomerCredit>();
    }
}
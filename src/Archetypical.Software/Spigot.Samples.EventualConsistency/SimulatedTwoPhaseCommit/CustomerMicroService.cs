using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class CustomerMicroService
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private readonly List<Customer> _customers;

        public CustomerMicroService(List<Customer> customers)
        {
            _customers = customers;
        }

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }
    }
}
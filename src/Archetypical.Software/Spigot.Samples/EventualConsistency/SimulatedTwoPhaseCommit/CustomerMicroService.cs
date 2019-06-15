using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class CustomerMicroService
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private readonly List<Customer> _customers;

        public CustomerMicroService()
        {
            _customers = new List<Customer>();
            Spigot<OrderPlacedEvent>.Open += OrderPlaced;
            Spigot<OrderCompletedEvent>.Open += OrderCompleted;
        }

        private void OrderPlaced(object sender, EventArrived<OrderPlacedEvent> e)
        {
            Task.Delay(Random.Next() % 2000).GetAwaiter().GetResult(); //simulate a lookup
            var customer = _customers.FirstOrDefault(x => x.CustomerId == e.EventData.CustomerId);
            if (customer == null)
            {
                Spigot<CreditResultEvent>.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Declined,
                    CustomerId = e.EventData.CustomerId,
                    OrderId = e.EventData.OrderId
                });
                return;
            }

            if (customer.AvailableLimit >= e.EventData.Amount)
            {
                customer.CustomerCredits.Add(new CustomerCredit
                {
                    Amount = e.EventData.Amount,
                    CreditStatus = CreditStatus.Hold,
                    OrderReference = e.EventData.OrderId
                });

                Spigot<CreditResultEvent>.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Hold,
                    CustomerId = e.EventData.CustomerId,
                    OrderId = e.EventData.OrderId
                });
            }
            else
            {
                Spigot<CreditResultEvent>.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Declined,
                    CustomerId = e.EventData.CustomerId,
                    OrderId = e.EventData.OrderId
                });
            }
        }

        private void OrderCompleted(object sender, EventArrived<OrderCompletedEvent> e)
        {
            var customer = _customers.FirstOrDefault(x => x.CustomerId == e.EventData.CustomerId);
            var item = customer?.CustomerCredits.FirstOrDefault(x => x.OrderReference == e.EventData.OrderId);
            if (item != null)
            {
                if (e.EventData.OrderStatus == OrderStatus.Completed)
                {
                    item.CreditStatus = CreditStatus.Approved;
                }

                if (e.EventData.OrderStatus == OrderStatus.Declined)
                {
                    item.CreditStatus = CreditStatus.Declined;
                }
            }
        }

        public void AddCustomer(Customer customer)
        {
            _customers.Add(customer);
        }
    }
}
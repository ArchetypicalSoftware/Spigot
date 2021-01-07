using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs
{
    public class OrderPlacedKnob : Knob<OrderPlacedEvent>
    {
        private readonly MessageSender<CreditResultEvent> _creditResultSender;
        private readonly List<Customer> _customers;
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public OrderPlacedKnob(
            Archetypical.Software.Spigot.Spigot spigot,
            MessageSender<CreditResultEvent> creditResultSender,
            List<Customer> customers,
        ILogger<OrderPlacedKnob> logger) : base(spigot, logger)
        {
            _creditResultSender = creditResultSender;
            _customers = customers;
        }

        protected override void HandleMessage(EventArrived<OrderPlacedEvent> message)
        {
            Task.Delay(Random.Next() % 2000).GetAwaiter().GetResult(); //simulate a lookup
            var customer = _customers.FirstOrDefault(x => x.CustomerId == message.EventData.CustomerId);
            if (customer == null)
            {
                _creditResultSender.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Declined,
                    CustomerId = message.EventData.CustomerId,
                    OrderId = message.EventData.OrderId
                });
                return;
            }

            if (customer.AvailableLimit >= message.EventData.Amount)
            {
                customer.CustomerCredits.Add(new CustomerCredit
                {
                    Amount = message.EventData.Amount,
                    CreditStatus = CreditStatus.Hold,
                    OrderReference = message.EventData.OrderId
                });

                _creditResultSender.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Hold,
                    CustomerId = message.EventData.CustomerId,
                    OrderId = message.EventData.OrderId
                });
            }
            else
            {
                _creditResultSender.Send(new CreditResultEvent()
                {
                    CreditStatus = CreditStatus.Declined,
                    CustomerId = message.EventData.CustomerId,
                    OrderId = message.EventData.OrderId
                });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs
{
    public class OrderCompletedKnob : Knob<OrderCompletedEvent>
    {
        private readonly List<Customer> _customers;

        public OrderCompletedKnob(
            Archetypical.Software.Spigot.Spigot spigot,
            List<Customer> customers,
            ILogger<OrderCompletedKnob> logger) : base(spigot, logger)
        {
            _customers = customers;
        }

        protected override void HandleMessage(EventArrived<OrderCompletedEvent> message)
        {
            var customer = _customers.FirstOrDefault(x => x.CustomerId == message.EventData.CustomerId);
            var item = customer?.CustomerCredits.FirstOrDefault(x => x.OrderReference == message.EventData.OrderId);
            if (item != null)
            {
                if (message.EventData.OrderStatus == OrderStatus.Completed)
                {
                    item.CreditStatus = CreditStatus.Approved;
                }

                if (message.EventData.OrderStatus == OrderStatus.Declined)
                {
                    item.CreditStatus = CreditStatus.Declined;
                }
            }
        }
    }

    public class ReadOnlyOrderCompletedKnob : Knob<OrderCompletedEvent>
    {
        public Action<EventArrived<OrderCompletedEvent>> Open { get; set; }

        public ReadOnlyOrderCompletedKnob(Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<OrderCompletedEvent>> logger) : base(spigot, logger)
        {
        }

        protected override void HandleMessage(EventArrived<OrderCompletedEvent> message)
        {
            Open?.Invoke(message);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Archetypical.Software.Spigot;
using Spigot.Samples.Common;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class TwoPhaseCommitDemonstrator : IDemonstrator
    {
        private readonly CustomerMicroService _customerMicroService;
        private readonly OrderCompletedKnob _orderCompletedKnob;
        private readonly OrderMicroService _orderMicroService;
        private readonly ReadOnlyOrderCompletedKnob _readOnlyOrderCompletedKnob;
        private static TextWriter _writer;

        public TwoPhaseCommitDemonstrator(
            CustomerMicroService customerMicroService,
            OrderCompletedKnob orderCompletedKnob,
            OrderMicroService orderMicroService,
            ReadOnlyOrderCompletedKnob readOnlyOrderCompletedKnob)
        {
            _customerMicroService = customerMicroService;
            _orderCompletedKnob = orderCompletedKnob;
            _orderMicroService = orderMicroService;
            _readOnlyOrderCompletedKnob = readOnlyOrderCompletedKnob;
        }

        public async Task GoAsync(TextWriter writer)
        {
            await Task.Yield();
            _writer = writer;
            Random random = new Random();
            _readOnlyOrderCompletedKnob.Open += OrderCompleted;

            var customer = new Customer
            {
                CreditLimit = 1000
            };
            _customerMicroService.AddCustomer(customer);
            var ids = new List<Guid>();
            for (int x = 0; x < 10; x++)
            {
                var order = new Order()
                {
                    CustomerId = customer.CustomerId,
                    Amount = random.Next(1, 500),
                };
                ids.Add(_orderMicroService.PlaceOrder(order));
                writer.WriteLine($"Sent {order.Id}");
            }
        }

        private static void OrderCompleted(EventArrived<OrderCompletedEvent> e)
        {
            _writer?.WriteLine($"\tOrder {e.EventData.OrderId} {e.EventData.OrderStatus} with amount {e.EventData.Amount}");
        }

        public void Describe(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
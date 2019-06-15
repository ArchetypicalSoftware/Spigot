using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using System;
using System.Collections.Generic;
using System.IO;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class Demonstrator : IDemonstrator
    {
        private static TextWriter _writer;

        public void Go(TextWriter writer)
        {
            _writer = writer;
            Random random = new Random();
            Archetypical.Software.Spigot.Spigot<OrderCompletedEvent>.Open += OrderCompleted;

            var orderService = new OrderMicroService();
            var customerService = new CustomerMicroService();

            var customer = new Customer
            {
                CreditLimit = 1000
            };
            customerService.AddCustomer(customer);
            var ids = new List<Guid>();
            for (int x = 0; x < 10; x++)
            {
                var order = new Order()
                {
                    CustomerId = customer.CustomerId,
                    Amount = random.Next(1, 500),
                };
                ids.Add(orderService.PlaceOrder(order));
                writer.WriteLine($"Sent {order.Id}");
            }
        }

        private static void OrderCompleted(object sender, Archetypical.Software.Spigot.EventArrived<OrderCompletedEvent> e)
        {
            _writer?.WriteLine($"\tOrder {e.EventData.OrderId} {e.EventData.OrderStatus} with amount {e.EventData.Amount}");
        }

        public void Describe(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class OrderMicroService
    {
        private List<Order> orders;

        public OrderMicroService()
        {
            orders = new List<Order>();
            Spigot<CreditResultEvent>.Open += CreditReserved;
        }

        private void CreditReserved(object sender, EventArrived<CreditResultEvent> e)
        {
            var order = orders.FirstOrDefault(x => x.Id == e.EventData.OrderId);
            if (order == null || order.Status != OrderStatus.Pending)
            {
                return;
            }

            order.Status = e.EventData.CreditStatus == CreditStatus.Hold ? OrderStatus.Completed : OrderStatus.Declined;
            Spigot<OrderCompletedEvent>.Send(new OrderCompletedEvent
            {
                Amount = order.Amount,
                CustomerId = order.CustomerId,
                OrderStatus = order.Status,
                OrderId = order.Id
            });
        }

        public Guid PlaceOrder(Order order)
        {
            order.Status = OrderStatus.Pending;
            orders.Add(order);
            Spigot<OrderPlacedEvent>.Send(new OrderPlacedEvent()
            {
                Amount = order.Amount,
                CustomerId = order.CustomerId,
                OrderId = order.Id
            });
            return order.Id;
        }

        public Order LookupOrder(Guid orderId)
        {
            return orders.FirstOrDefault(x => x.Id == orderId);
        }
    }
}
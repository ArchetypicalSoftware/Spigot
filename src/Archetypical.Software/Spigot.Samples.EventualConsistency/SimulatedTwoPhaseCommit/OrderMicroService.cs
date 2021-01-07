using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Data;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit
{
    public class OrderMicroService
    {
        private readonly MessageSender<OrderPlacedEvent> _orderPlacedSender;
        private readonly MessageSender<OrderCompletedEvent> _orderCompletedSender;
        private readonly CreditResultKnob _creditResultKnob;
        private List<Order> orders;

        public OrderMicroService(
            MessageSender<OrderPlacedEvent> orderPlacedSender,
            MessageSender<OrderCompletedEvent> orderCompletedSender,
            CreditResultKnob creditResultKnob
        )
        {
            _orderPlacedSender = orderPlacedSender;
            _orderCompletedSender = orderCompletedSender;
            _creditResultKnob = creditResultKnob;
            orders = new List<Order>();
            _creditResultKnob.Open += CreditReserved;
        }

        private void CreditReserved(EventArrived<CreditResultEvent> e)
        {
            var order = orders.FirstOrDefault(x => x.Id == e.EventData.OrderId);
            if (order == null || order.Status != OrderStatus.Pending)
            {
                return;
            }

            order.Status = e.EventData.CreditStatus == CreditStatus.Hold ? OrderStatus.Completed : OrderStatus.Declined;
            _orderCompletedSender.Send(new OrderCompletedEvent
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
            _orderPlacedSender.Send(new OrderPlacedEvent()
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
using System;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Events;

namespace Spigot.Samples.EventualConsistency.SimulatedTwoPhaseCommit.Knobs
{
    public class CreditResultKnob : Knob<CreditResultEvent>
    {
        public Action<EventArrived<CreditResultEvent>> Open { get; set; }

        public CreditResultKnob(Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<CreditResultEvent>> logger) : base(spigot, logger)
        {
        }

        protected override void HandleMessage(EventArrived<CreditResultEvent> message)
        {
            Open?.Invoke(message);
        }
    }
}
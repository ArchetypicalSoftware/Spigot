using System;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.MaterializedView.Data;

namespace Spigot.Samples.EventualConsistency.MaterializedView
{
    public class RecordKnob : Knob<Record>
    {
        public RecordKnob(Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<Record>> logger) : base(spigot, logger)
        {
        }

        public Action<EventArrived<Record>> Open { get; internal set; }

        protected override void HandleMessage(EventArrived<Record> message)
        {
            //Demonstrates an easy transition from v1 to v2 of Spigot
            Open?.Invoke(message);
        }
    }
}
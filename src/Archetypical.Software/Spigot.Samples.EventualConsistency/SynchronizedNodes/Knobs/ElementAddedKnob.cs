using System;
using System.Collections.Generic;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Knobs
{
    public class ElementAddedKnob : Knob<ElementAddedEvent>
    {
        private readonly Dictionary<Guid, DataElement> _indexedDataElements;
        private readonly List<DataElement> _conflicts;

        public ElementAddedKnob(
            Archetypical.Software.Spigot.Spigot spigot,
            ILogger<Knob<ElementAddedEvent>> logger,
            Dictionary<Guid, DataElement> indexedDataElements,
            List<DataElement> conflicts) : base(spigot, logger)
        {
            _indexedDataElements = indexedDataElements;
            _conflicts = conflicts;
        }

        protected override void HandleMessage(EventArrived<ElementAddedEvent> message)
        {
            var newElement = message.EventData.DataElement;

            if (_indexedDataElements.ContainsKey(newElement.GuidIdentifier))
            {
                _conflicts.Add(newElement);
                //raise event
                return;
            }

            _indexedDataElements[newElement.GuidIdentifier] = newElement;
        }
    }
}
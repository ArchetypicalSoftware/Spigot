using System;
using System.Collections.Generic;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Knobs
{
    public class ElementUpdatedKnob : Knob<ElementUpdatedEvent>
    {
        private readonly Dictionary<Guid, DataElement> _indexedDataElements;
        private readonly List<DataElement> _conflicts;
        private readonly MessageSender<ReSyncRequestEvent> _sender;

        public ElementUpdatedKnob(
            Archetypical.Software.Spigot.Spigot spigot,
            ILogger<Knob<ElementUpdatedEvent>> logger,
            Dictionary<Guid, DataElement> indexedDataElements,
            List<DataElement> conflicts,
            MessageSender<ReSyncRequestEvent> sender) : base(spigot, logger)
        {
            _indexedDataElements = indexedDataElements;
            _conflicts = conflicts;
            _sender = sender;
        }

        protected override void HandleMessage(EventArrived<ElementUpdatedEvent> message)
        {
            //if (message.EventData.Instance.Equals(Name))
            //{
            //    return;
            //}

            if (!_indexedDataElements.ContainsKey(message.EventData.ElementId))
            {
                // I need the latest
                _sender.Send(new ReSyncRequestEvent { GuidIdentifier = message.EventData.ElementId });
                return;
            }

            var currentItem = _indexedDataElements[message.EventData.ElementId];
            ItemComparer<DataElement>.ApplyDifferences(currentItem, message.EventData.ChangedProperties);
        }
    }
}
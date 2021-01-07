using System;
using System.Collections.Generic;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Knobs
{
    public class SyncResponseKnob : Knob<SyncResponseEvent>
    {
        private readonly ILogger<SyncResponseKnob> _logger;
        private readonly Dictionary<Guid, DataElement> _indexedDataElements;
        private readonly List<DataElement> _conflicts;

        public SyncResponseKnob(Archetypical.Software.Spigot.Spigot spigot,
            ILogger<SyncResponseKnob> logger,
            Dictionary<Guid, DataElement> indexedDataElements,
            List<DataElement> conflicts) : base(spigot, logger)
        {
            _logger = logger;
            _indexedDataElements = indexedDataElements;
            _conflicts = conflicts;
        }

        protected override void HandleMessage(EventArrived<SyncResponseEvent> message)
        {
            if (!_indexedDataElements.ContainsKey(message.EventData.DataElement.GuidIdentifier))
            {
                _indexedDataElements[message.EventData.DataElement.GuidIdentifier] = message.EventData.DataElement;
            }
            else
            {
                var current = _indexedDataElements[message.EventData.DataElement.GuidIdentifier];
                if (current.LastChanged < message.EventData.DataElement.LastChanged)
                {
                    _indexedDataElements[message.EventData.DataElement.GuidIdentifier] = message.EventData.DataElement;
                }
            }
        }
    }
}
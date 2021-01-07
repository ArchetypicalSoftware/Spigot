using System;
using System.Collections.Generic;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Knobs
{
    public class ReSyncRequestKnob : Knob<ReSyncRequestEvent>
    {
        private readonly MessageSender<SyncResponseEvent> _sender;
        private readonly Dictionary<Guid, DataElement> _indexedDataElements;

        public ReSyncRequestKnob(Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<ReSyncRequestEvent>> logger, MessageSender<SyncResponseEvent> sender,
            Dictionary<Guid, DataElement> indexedDataElements) : base(spigot, logger)
        {
            _sender = sender;
            _indexedDataElements = indexedDataElements;
        }

        protected override void HandleMessage(EventArrived<ReSyncRequestEvent> message)
        {
            if (_indexedDataElements.ContainsKey(message.EventData.GuidIdentifier))
            {
                _sender.Send(new SyncResponseEvent
                { DataElement = _indexedDataElements[message.EventData.GuidIdentifier] });
            }
        }
    }
}
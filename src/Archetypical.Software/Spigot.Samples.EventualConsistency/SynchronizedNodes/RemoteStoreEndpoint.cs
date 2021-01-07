using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes
{
    public class RemoteStoreEndpoint
    {
        private readonly MessageSender<ElementAddedEvent> _addedSender;
        private readonly MessageSender<ElementUpdatedEvent> _updatedSender;
        private readonly Dictionary<Guid, DataElement> _indexedDataElements;
        private readonly List<DataElement> _conflicts;

        public RemoteStoreEndpoint(
            MessageSender<ElementAddedEvent> addedSender,
            MessageSender<ElementUpdatedEvent> updatedSender,
            Dictionary<Guid, DataElement> indexedDataElements,
            List<DataElement> conflicts
            )
        {
            _addedSender = addedSender;
            _updatedSender = updatedSender;
            _indexedDataElements = indexedDataElements;
            _conflicts = conflicts;
        }

        internal static DataElement Clone(DataElement element)
        {
            return JsonConvert.DeserializeObject<DataElement>(JsonConvert.SerializeObject(element));
        }

        public void AddOrUpdateDataElement(DataElement element)
        {
            if (!_indexedDataElements.ContainsKey(element.GuidIdentifier))
            {
                _indexedDataElements[element.GuidIdentifier] = element;
                _addedSender.Send(new ElementAddedEvent
                {
                    Instance = Name,
                    DataElement = Clone(element)
                });
                return;
            }

            var currentItem = _indexedDataElements[element.GuidIdentifier];
            var difference = ItemComparer<DataElement>.Compare(element, currentItem);
            if (difference.Any())
            {
                _updatedSender.Send(new ElementUpdatedEvent
                {
                    Instance = Name,
                    ElementId = element.GuidIdentifier,
                    ChangedProperties = difference
                });
            }
            _indexedDataElements[element.GuidIdentifier] = element;
        }

        public DataElement GetDateElement(Guid elementId)
        {
            if (_indexedDataElements.ContainsKey(elementId))
            {
                return _indexedDataElements[elementId];
            }
            return null;
        }

        public string Name { get; } = $"Instance {Guid.NewGuid()}";
    }
}
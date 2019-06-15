using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes
{
    public class RemoteStoreEndpoint
    {
        public RemoteStoreEndpoint()
        {
            Spigot<ElementAddedEvent>.Open += NewElementsAdded;
            Spigot<ElementUpdatedEvent>.Open += ElementsUpdated;
            Spigot<ReSyncRequestEvent>.Open += ReSyncRequest;
            Spigot<SyncResponseEvent>.Open += SyncResponse;
        }

        private void SyncResponse(object sender, EventArrived<SyncResponseEvent> e)
        {
            if (e.EventData.Instance.Equals(Name))
            {
                return;
            }

            if (!IndexedDataElements.ContainsKey(e.EventData.DataElement.GuidIdentifier))
            {
                IndexedDataElements[e.EventData.DataElement.GuidIdentifier] = e.EventData.DataElement;
            }
            else
            {
                var current = IndexedDataElements[e.EventData.DataElement.GuidIdentifier];
                if (current.LastChanged < e.EventData.DataElement.LastChanged)
                {
                    IndexedDataElements[e.EventData.DataElement.GuidIdentifier] = e.EventData.DataElement;
                }
            }
        }

        private void ReSyncRequest(object sender, EventArrived<ReSyncRequestEvent> e)
        {
            if (e.EventData.Instance.Equals(Name))
            {
                return;
            }

            if (IndexedDataElements.ContainsKey(e.EventData.GuidIdentifier))
            {
                Spigot<SyncResponseEvent>.Send(new SyncResponseEvent
                { DataElement = IndexedDataElements[e.EventData.GuidIdentifier] });
            }
        }

        private void ElementsUpdated(object sender, EventArrived<ElementUpdatedEvent> e)
        {
            if (e.EventData.Instance.Equals(Name))
            {
                return;
            }

            if (!IndexedDataElements.ContainsKey(e.EventData.ElementId))
            {
                // I need the latest
                Spigot<ReSyncRequestEvent>.Send(new ReSyncRequestEvent { GuidIdentifier = e.EventData.ElementId });
                return;
            }

            var currentItem = IndexedDataElements[e.EventData.ElementId];
            ItemComparer<DataElement>.ApplyDifferences(currentItem, e.EventData.ChangedProperties);
        }

        private void NewElementsAdded(object sender, EventArrived<ElementAddedEvent> e)
        {
            if (e.EventData.Instance.Equals(Name))
            {
                return;
            }
            var newElement = e.EventData.DataElement;

            if (IndexedDataElements.ContainsKey(newElement.GuidIdentifier))
            {
                Conflicts.Add(newElement);
                //raise event
                return;
            }

            IndexedDataElements[newElement.GuidIdentifier] = newElement;
        }

        public void AddOrUpdateDataElement(DataElement element)
        {
            if (!IndexedDataElements.ContainsKey(element.GuidIdentifier))
            {
                IndexedDataElements[element.GuidIdentifier] = element;
                Spigot<ElementAddedEvent>.Send(new ElementAddedEvent
                {
                    Instance = Name,
                    DataElement = Demonstrator.Clone(element)
                });
                return;
            }

            var currentItem = IndexedDataElements[element.GuidIdentifier];
            var difference = ItemComparer<DataElement>.Compare(element, currentItem);
            if (difference.Any())
            {
                Spigot<ElementUpdatedEvent>.Send(new ElementUpdatedEvent
                {
                    Instance = Name,
                    ElementId = element.GuidIdentifier,
                    ChangedProperties = difference
                });
            }
            IndexedDataElements[element.GuidIdentifier] = element;
        }

        public DataElement GetDateElement(Guid elementId)
        {
            if (IndexedDataElements.ContainsKey(elementId))
            {
                return IndexedDataElements[elementId];
            }
            return null;
        }

        public Dictionary<Guid, DataElement> IndexedDataElements { get; set; } = new Dictionary<Guid, DataElement>();
        public List<DataElement> Conflicts { get; set; } = new List<DataElement>();
        public string Name { get; } = $"Instance {Guid.NewGuid()}";
    }
}
using System;
using System.Collections.Generic;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    public class ElementUpdatedEvent : BaseElementEvent
    {
        public Guid ElementId { get; set; }
        public List<Property> ChangedProperties { get; set; } = new List<Property>();
    }
}
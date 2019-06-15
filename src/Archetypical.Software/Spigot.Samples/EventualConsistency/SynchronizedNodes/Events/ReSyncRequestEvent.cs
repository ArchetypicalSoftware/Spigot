using System;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    public class ReSyncRequestEvent : BaseElementEvent
    {
        public Guid GuidIdentifier { get; set; }
    }
}
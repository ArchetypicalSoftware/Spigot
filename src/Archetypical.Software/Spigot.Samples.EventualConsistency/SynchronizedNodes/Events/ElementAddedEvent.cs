using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    public class ElementAddedEvent : BaseElementEvent
    {
        public DataElement DataElement { get; set; }
    }
}
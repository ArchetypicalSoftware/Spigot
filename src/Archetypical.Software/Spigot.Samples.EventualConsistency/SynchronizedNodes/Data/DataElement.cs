using System;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Data
{
    public class DataElement
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public Guid GuidIdentifier { get; set; }
        public State State { get; set; }
        public DateTimeOffset LastChanged { get; set; }

        public override string ToString()
        {
            return $"{GetHashCode()}";
        }
    }
}
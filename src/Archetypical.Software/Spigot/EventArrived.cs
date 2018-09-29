using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Archetypical.Software.Spigot
{
    
    public class Envelope : EventArgs
    { 
        public string Event { get; set; }
        public string FQN { get; set; }
        public Guid MessageIdentifier { get; set; }
        public Sender Sender { get; set; }
        public List<Header> Headers { get; set; }
        public byte[] SerializedEventData { get; set; }
    }

    public class EventArrived<T>
    {
        public T GetT { get; set; }
        public Context Context { get; set; }
    }

    public class Context
    {
        public Sender Sender { get; set; }
        public List<Header> Headers { get; set; }
}
}
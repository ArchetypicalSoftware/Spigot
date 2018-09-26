using System;
using System.Collections.Generic;

namespace Archetypical.Software.Spigot
{
    public class EventArrived<T> : EventArgs
    {
        public Guid MessageIdentifier { get; set; }
        public Sender Sender { get; set; }
        public List<Header> Headers { get; set; }
        public T EventData { get; set; }
    }
}
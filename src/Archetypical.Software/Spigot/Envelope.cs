using System;
using System.Collections.Generic;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// The internal event that is raised and the envelope for the message serialized
    /// </summary>
    public class Envelope : EventArgs
    {
        public Envelope()
        {
            Headers = new Headers();
        }
        /// <summary>
        /// The name of the event. This corresponds to the simple type name of the event sent
        /// </summary>
        public string Event { get; set; }
        /// <summary>
        /// The fully qualified name of the type being sent
        /// </summary>
        public string FQN { get; set; }
        /// <summary>
        /// A unique message identifier for each sent message
        /// </summary>
        public Guid MessageIdentifier { get; set; }
        /// <summary>
        /// Metadata about the sending application
        /// </summary>
        public Sender Sender { get; set; }
        /// <summary>
        /// A header collection of additional information 
        /// </summary>
        public Headers Headers { get; set; }
        /// <summary>
        /// The serialized event data
        /// </summary>
        public byte[] SerializedEventData { get; set; }
    }
}
using System.Collections.Generic;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// The sending context of the event being sent
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Metadata about the sender <see cref="Sender"/>
        /// </summary>
        public Sender Sender { get; set; }
         

        /// <summary>
        /// Additional headers sent by the <see cref="Sender"/>
        /// </summary>
        public Headers  Headers { get; set;}
    }

       
}
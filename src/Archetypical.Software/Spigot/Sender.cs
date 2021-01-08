using System;
using System.Linq;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Information about the sending application
    /// </summary>
    public class Sender
    {
        public Sender(Uri cloudEventSource)
        {
            var split = cloudEventSource.ToString().Split(':');

            if (
                split.Length != 4
                || split.First() != "urn"
                || !split[1].StartsWith("spigot-")
                || !int.TryParse(split[1].Split('-').Last(), out var processId)
                || !Guid.TryParse(split[3], out var instanceGuid))
            {
                Name = cloudEventSource.ToString();
            }
            else
            {
                ProcessId = processId;
                Name = split[2];
                InstanceIdentifier = instanceGuid;
            }
        }

        /// <summary>
        /// The Process Identifier <see cref="System.Environment.CurrentManagedThreadId"/> of the sender
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// The Senders friendly name as is set in <see cref="Spigot.ApplicationName"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique instance identifier to identifier each sender if deployed in a multi-instance environment.See <see cref="Spigot.InstanceIdentifier"/>
        /// </summary>
        public Guid InstanceIdentifier { get; set; }
    }
}
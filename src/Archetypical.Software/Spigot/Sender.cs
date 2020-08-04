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
            int processId;
            Guid instanceGuid;
            if (split.Length != 3 || !split.First().StartsWith("spigot-") || !int.TryParse(split[0].Substring(7), out processId) || !Guid.TryParse(split[2], out instanceGuid))
            {
                Name = "Unknown sender.";
            }
            else
            {
                ProcessId = processId;
                Name = split[1];
                InstanceIdentifier = instanceGuid;
            }
        }

        /// <summary>
        /// The Process Identifier <see cref="System.Environment.CurrentManagedThreadId"/> of the sender
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// The Senders friendly name as is set in <see cref="SpigotSettings.ApplicationName"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique instance identifier to identifier each sender if deployed in a multi-instance environment.See <see cref="SpigotSettings.InstanceIdentifier"/>
        /// </summary>
        public Guid InstanceIdentifier { get; set; }
    }
}
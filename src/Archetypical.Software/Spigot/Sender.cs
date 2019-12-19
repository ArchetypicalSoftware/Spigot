using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Information about the sending application
    /// </summary>
    public class Sender
    {
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
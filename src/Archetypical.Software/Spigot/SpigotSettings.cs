using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// A collection of settings that control the Spigots in this domain
    /// </summary>
    public class SpigotSettings
    {
        internal Func<ISpigotSerializer> SerializerFactory;
        internal Func<ISpigotStream> StreamFactory;

        public SpigotSettings()
        {
            AddSerializer(new DefaultDataContractSerializer());
            AddStream(new LocalStream());
            ApplicationName = Environment.CommandLine;
        }

        /// <summary>
        /// Add a custom implementation of an <see cref="ISpigotSerializer"/>
        /// </summary>
        /// <param name="serializer">An implementation of <see cref="ISpigotSerializer"/></param>
        public void AddSerializer(ISpigotSerializer serializer)
        {
            SerializerFactory = () => serializer;
        }

        /// <summary>
        /// Add a custom implementation of an <see cref="ISpigotStream"/>
        /// </summary>
        /// <param name="stream">An implementation of <see cref="ISpigotStream"/></param>
        public void AddStream(ISpigotStream stream)
        {
            StreamFactory = () => stream;
        }

        /// <summary>
        /// The name of the current application.
        /// </summary>
        /// <remarks>Set this to indicate which business-friendly product is hosting these spigots</remarks>
        /// <example>Inventory Service, Billing Service, Monitoring Application</example>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The unique identifier of this instance. 
        /// </summary>
        public Guid InstanceIdentifier { get; } = Guid.NewGuid();


        public Action<Envelope> BeforeSend { get; set; }
        public Action<Envelope> AfterReceive { get; set; }
    }
}
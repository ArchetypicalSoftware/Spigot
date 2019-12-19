using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot.Extensions
{
    /// <inheritdoc />
    public class SpigotBuilder : ISpigotBuilder
    {
        /// <inheritdoc />
        public IServiceCollection Services { get; set; }

        /// <inheritdoc />
        public IConfiguration Configuration { get; set; }

        /// <inheritdoc />
        public ISpigotSerializer Serializer { get; set; } = new DefaultDataContractSerializer();

        /// <inheritdoc />
        public ISpigotStream Stream { get; set; } = new LocalStream();

        /// <inheritdoc />
        public string ApplicationName { get; set; }

        /// <inheritdoc />
        public Guid InstanceIdentifier { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public Action<Envelope> BeforeSend { get; set; }

        /// <inheritdoc />
        public Action<Envelope> AfterReceive { get; set; }

        /// <inheritdoc />
        public Resilience Resilience { get; set; }

        /// <inheritdoc />
        public void Build()
        {
            var spigot = Services.BuildServiceProvider().GetService<Spigot>();
            spigot.Setup(this);
        }

        public List<Type> Knobs { get; set; } = new List<Type>();
    }
}
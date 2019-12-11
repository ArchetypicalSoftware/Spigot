using Archetypical.Software.Spigot.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Spigot.LoadTests")]

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// A Spigot allows access to get and send streams of strongly typed messages
    /// </summary>
    public class Spigot
    {
        private readonly ILogger<Spigot> _logger;

        internal readonly ConcurrentDictionary<string, Action<CloudEvent>> Knobs =
            new ConcurrentDictionary<string, Action<CloudEvent>>();

        public Spigot(ILogger<Spigot> logger)
        {
            _logger = logger;
        }

        private bool _initialized;
        internal ISpigotSerializer Serializer;
        internal ISpigotStream Stream;
        internal string ApplicationName { get; set; }
        internal Action<Envelope> AfterReceive { get; set; }
        internal Resilience Resilience { get; set; }
        internal Action<Envelope> BeforeSend { get; set; }
        internal Guid InstanceIdentifier { get; set; }
        internal JsonEventFormatter EnvelopeFormatter = new JsonEventFormatter();

        /// <summary>
        /// Allows for the configuration of the Spigot via an instance of <see cref="SpigotSettings"/>
        /// </summary>
        internal void Setup(ISpigotBuilder builder)
        {
            if (_initialized)
            {
                _logger.LogInformation("Unbinding previous spigot configuration for new settings");
                Stream.DataArrived -= Spigot_DataArrived;

                Stream = null;
                AfterReceive = null;
                BeforeSend = null;
                InstanceIdentifier = Guid.Empty;
                ApplicationName = string.Empty;
                Resilience = null;
            }

            AfterReceive = builder.AfterReceive;
            BeforeSend = builder.BeforeSend;
            InstanceIdentifier = builder.InstanceIdentifier;
            ApplicationName = builder.ApplicationName;
            Resilience = builder.Resilience ?? new Resilience();
            _initialized = true;
            builder.Services.AddSingleton(this);
            var provider = builder.Services.BuildServiceProvider();
            Stream = provider.GetService<ISpigotStream>() ?? new LocalStream();
            Serializer = provider.GetService<ISpigotSerializer>() ?? new DefaultJsonSerializer();
            Stream.DataArrived += Spigot_DataArrived;
            foreach (var knobType in builder.Knobs)
            {
                var knob = provider.GetService(knobType) as Knob;
                builder.Services.AddSingleton(knob);
                knob?.Register();
            }
        }

        internal void Register<T>(Knob<T> knob) where T : class, new()
        {
            if (!_initialized)
            {
                throw new NotSupportedException(
                    "Spigot has not been initialized. Use the AddSpigot extension method on IServiceCollection");
            }

            var typeName = typeof(T).Name;
            Knobs[typeName] = message =>
            {
                if (!(message is Envelope arrived)) return;
                knob.HandleMessage(arrived);
            };
            _logger.LogTrace($"Spigot Callback Registered for {typeName}");
        }

        private void Spigot_DataArrived(object sender, byte[] e)
        {
            var envelope = EnvelopeFormatter.DecodeStructuredEvent(e, null);
            Knobs[envelope.Type].Invoke(envelope);
        }
    }
}
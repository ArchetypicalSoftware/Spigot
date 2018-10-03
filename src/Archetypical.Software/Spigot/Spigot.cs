using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Spigot.LoadTests")]
namespace Archetypical.Software.Spigot
{
    public static class Spigot<T> where T : class, new()
    {
        private static readonly ILogger Logger;
        static Spigot()
        {
            Spigot.Register(typeof(T), args =>
            {
                if (!(args is Envelope arrived)) return;
                Logger.LogTrace("Envelope of type [{0}] arrived with id {1}", arrived.Event,arrived.MessageIdentifier);
                Dispatch(arrived);
            });
            Logger = Spigot.Settings.LoggerFactory.CreateLogger(typeof(Spigot<T>));
        }

        /// <summary>
        /// Gets invoked whenever an instance of T is received from the <see cref="ISpigotStream"/>
        /// </summary>
        public static event EventHandler<EventArrived<T>> Open;

       

        /// <summary>
        /// Allows you to send an instance of T to the <see cref="ISpigotStream"/>
        /// </summary>
        /// <param name="eventData">The data to be sent over</param>
        public static void Send(T eventData)
        {
            var wrapper = new Envelope
            {
                SerializedEventData = Spigot.Settings.SerializerFactory().Serialize(eventData),
                Event = typeof(T).Name,
                FQN = typeof(T).FullName,
                MessageIdentifier = Guid.NewGuid(),
                Sender = new Sender
                {
                    ProcessId = Environment.CurrentManagedThreadId,
                    Name = Spigot.Settings.ApplicationName,
                    InstanceIdentifier = Spigot.Settings.InstanceIdentifier
                }
            };
            Spigot.Settings.BeforeSend?.Invoke(wrapper);
            Logger.LogTrace("Sending [{0}] with id {1}", wrapper.Event, wrapper.MessageIdentifier);
            var bytes = Spigot.Settings.SerializerFactory().Serialize(wrapper);
            Spigot.Settings.Resilience.Sending.Execute(() =>
                {
                    Logger.LogTrace("Sending using resilience.");
                    Spigot.Settings.StreamFactory().TrySend(bytes);
                }
                );
            
        }

        private static void Dispatch(Envelope e)
        {
            if (Open == null) return;
            Spigot.Settings.AfterReceive?.Invoke(e);
            var raisedEvent = new EventArrived<T>
            {
                EventData = Spigot.Settings.SerializerFactory().Deserialize<T>(e.SerializedEventData),
                Context = new Context
                {
                    Headers = e.Headers,
                    Sender = e.Sender
                }
            };
            Logger.LogTrace($"Received {e.Event} message from stream with id {e.MessageIdentifier}");
            Open?.GetInvocationList().ToList().ForEach(del =>
            {
                try
                {
                    del.DynamicInvoke(Spigot.Settings.StreamFactory(), raisedEvent);
                }
                catch (Exception)
                {
                    //Log
                }
            });
        }
    }

    public static class Spigot
    {
        internal static SpigotSettings Settings = new SpigotSettings();
        private static readonly ConcurrentDictionary<string, Action<EventArgs>> Knobs = new ConcurrentDictionary<string, Action<EventArgs>>();
        private static bool _initialized;
        private static ILogger Logger;

        /// <summary>
        /// Allows for the configuration of the Spigot via an instance of <see cref="SpigotSettings"/>
        /// </summary>
        /// <param name="settingsBuilder">A delegate that will pass an instance of <see cref="SpigotSettings"/> with default values that can be overwritten by the implementer</param>
        public static void Setup(Action<SpigotSettings> settingsBuilder)
        {
            if (_initialized)
            {
                Settings = new SpigotSettings();
                Settings.StreamFactory().DataArrived -= Spigot_DataArrived;
                Logger?.LogWarning("Spigot Settings were already configured and settings are being overwritten");
            }
            
            settingsBuilder(Settings);
            Settings.StreamFactory().DataArrived += Spigot_DataArrived;
            _initialized = true;
        }

        internal static void Register(Type type, Action<EventArgs> spigotCallback)
        {
            if (!_initialized)
            {
                Setup(w => { /*Take the defaults*/ });
                
            }

            if (Logger == null)
            {
                Logger = Settings.LoggerFactory.CreateLogger(typeof(Spigot));
            }
            Knobs[type.Name] = spigotCallback;
            Logger.LogTrace("Spigot Callback Registered for {0}", type.Name);
        }

        private static void Spigot_DataArrived(object sender, byte[] e)
        {
            var envelope = Settings.SerializerFactory().Deserialize<Envelope>(e);
            Knobs[envelope.Event].Invoke(envelope);
        }
    }
}
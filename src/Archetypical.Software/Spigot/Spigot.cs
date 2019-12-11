using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using Archetypical.Software.Spigot.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
                Logger.LogTrace("Envelope of type [{0}] arrived with id {1}", arrived.Event, arrived.MessageIdentifier);
                Dispatch(arrived);
            });
            Logger = Spigot.LoggerFactory.CreateLogger(typeof(Spigot<T>));
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
                SerializedEventData = Spigot.Serializer.Serialize(eventData),
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
            //Send it to all listeners in the same process space
            Dispatch(wrapper);

            var bytes = Spigot.Serializer.Serialize(wrapper);

            Spigot.Settings.Resilience.Sending.Execute(() =>
                    {
                        Logger.LogTrace("Sending using resilience.");
                        var result = Spigot.Stream?.TrySend(bytes);
                        if (!result.GetValueOrDefault())
                        {
                            throw new Exception("Sending exception");
                        }
                    });
        }

        private static void Dispatch(Envelope e)
        {
            if (Open == null) return;
            Spigot.Settings.AfterReceive?.Invoke(e);
            var raisedEvent = new EventArrived<T>
            {
                EventData = Spigot.Serializer.Deserialize<T>(e.SerializedEventData),
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
                    del.DynamicInvoke(Spigot.Stream, raisedEvent);
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
        internal static SpigotSettings Settings;
        private static readonly ConcurrentDictionary<string, Action<EventArgs>> Knobs = new ConcurrentDictionary<string, Action<EventArgs>>();
        private static bool _initialized;
        internal static ILogger Logger;
        internal static ILoggerFactory LoggerFactory;
        internal static ISpigotSerializer Serializer;
        internal static ISpigotStream Stream;

        /// <summary>
        /// Allows for the configuration of the Spigot via an instance of <see cref="SpigotSettings"/>
        /// </summary>
        internal static void Setup(ISpigotBuilder builder)
        {
            if (_initialized)
            {
                Logger?.LogInformation("Unbinding previous spigot configuration for new settings");
                Stream.DataArrived -= Spigot_DataArrived;
                Settings = null;
                LoggerFactory = null;
                Logger = null;
                Serializer = null;
                Stream = null;
            }
            Settings = builder.Settings;
            LoggerFactory = builder.LoggerFactory ?? NullLoggerFactory.Instance;
            Logger = LoggerFactory?.CreateLogger(typeof(Spigot));
            Serializer = builder.SerializerFactory;
            Stream = builder.StreamFactory;
            Stream.DataArrived += Spigot_DataArrived;
            _initialized = true;
        }

        internal static void Register(Type type, Action<EventArgs> spigotCallback)
        {
            if (!_initialized)
            {
                throw new NotSupportedException("Spigot has not been initialized. Use the AddSpigot extension method on IServiceCollection");
            }

            Knobs[type.Name] = spigotCallback;
            Logger.LogTrace("Spigot Callback Registered for {0}", type.Name);
        }

        private static void Spigot_DataArrived(object sender, byte[] e)
        {
            var envelope = Serializer.Deserialize<Envelope>(e);
            Knobs[envelope.Event].Invoke(envelope);
        }
    }
}
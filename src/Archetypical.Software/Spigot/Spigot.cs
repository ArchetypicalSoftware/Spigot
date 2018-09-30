using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Archetypical.Software.Spigot
{
    public static class Spigot<T> where T : class, new()
    {
        static Spigot()
        {
            Spigot.Register(typeof(T), args =>
            {
                if (args is Envelope arrived)
                    Dispatch(arrived);
            });
        }

        public static event EventHandler<EventArrived<T>> Open;

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
            var bytes = Spigot.Settings.SerializerFactory().Serialize(wrapper);
            Spigot.Settings.StreamFactory().TrySend(bytes);
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
            Open?.GetInvocationList().AsParallel().ForAll(del =>
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
        private static bool _initialized;
        private static readonly ConcurrentDictionary<string, Action<EventArgs>> Knobs = new ConcurrentDictionary<string, Action<EventArgs>>();

        public static void Setup(Action<SpigotSettings> settingsBuilder)
        {
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
            Knobs[type.Name] = spigotCallback;
        }

        private static void Spigot_DataArrived(object sender, byte[] e)
        {
            var envelope = Settings.SerializerFactory().Deserialize<Envelope>(e);
            Knobs[envelope.Event].Invoke(envelope);
        }
    }
}
using System;
using System.Collections.Concurrent;

namespace Archetypical.Software.Spigot
{
    public static class Spigot<T> where T:class,new()
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
                    Name = Spigot.Settings.ApplicationName
                }
            };

            var bytes = Spigot.Settings.SerializerFactory().Serialize(wrapper);
            Spigot.Settings.StreamFactory().TrySend(bytes);
        }

        private static void Dispatch(Envelope e)
        {
            if (Open != null)
            {
                var raisedEvent = new EventArrived<T>();
                raisedEvent.GetT = Spigot.Settings.SerializerFactory().Deserialize<T>(e.SerializedEventData);
                Open?.Invoke(Spigot.Settings.StreamFactory(), raisedEvent);
            }
        }
    }

    public static class Spigot
    {
        private static ConcurrentDictionary<string, Action<EventArgs>> Knobs = new ConcurrentDictionary<string, Action<EventArgs>>();
        private static bool initialized = false;
        internal static SpigotSettings Settings = new SpigotSettings();
        public static void Setup(Action<SpigotSettings> settingsBuilder) {
            settingsBuilder(Settings);
            Settings.StreamFactory().DataArrived += Spigot_DataArrived;
            initialized = true;
        }

        private static void Spigot_DataArrived(object sender, byte[] e)
        {
            var envelope = Settings.SerializerFactory().Deserialize<Envelope>(e);
            Knobs[envelope.Event].Invoke(envelope);
        }

        internal static void Register(Type type, Action<EventArgs> spigotCallback)
        {
            if (!initialized)
            {
                Setup(w => { });
            }
            Knobs[type.Name] = spigotCallback;
        }
    }
}
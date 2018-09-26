using System;

namespace Archetypical.Software.Spigot
{
    public static class Spigot<T>
    {
        public static event EventHandler<EventArrived<T>> Open;

        public static void Send(T eventData)
        {

        }
    }

    public static class Spigot
    {
        public static void Setup(Action<SpigotSettings> settings) {
            var setting = new SpigotSettings();
            settings(setting);
        }
    }
}
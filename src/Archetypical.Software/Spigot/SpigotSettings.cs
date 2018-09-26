using System;

namespace Archetypical.Software.Spigot
{
    public class SpigotSettings
    {
        Func<ISpigotSerializer> serializerFactory = () => null;
        Func<ISpigotStream> streamFactory = () => null;

        public void AddSerializer(ISpigotSerializer serializer)
        {
            serializerFactory = () => serializer;
        }
        public void AddStream(ISpigotStream stream)
        {
            streamFactory = () => stream;
        }
    }
}
using System;

namespace Archetypical.Software.Spigot
{
    public interface ISpigotStream
    {

        bool TrySend(byte[] data);

        event EventHandler<byte[]> DataArrived;

    }
}
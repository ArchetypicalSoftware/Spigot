using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// This is a simple callback stream for all applications in a single App Domain.
    /// </summary>
    internal class LocalStream : ISpigotStream
    {
        /// <inheritdoc />
        public bool TrySend(byte[] data)
        {
            return true;
        }

        /// <inheritdoc />
        public event EventHandler<byte[]> DataArrived;
    }
}
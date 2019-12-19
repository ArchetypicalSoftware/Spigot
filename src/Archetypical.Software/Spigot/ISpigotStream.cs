using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Represents a simple generic data stream that can be used to send and receive data
    /// The implementation should handle any additional enveloping, persistence and fault tolerance
    /// needed.
    /// </summary>
    public interface ISpigotStream : IDisposable
    {
        /// <summary>
        /// Tries to send an array of bytes over the stream
        /// </summary>
        /// <param name="data">The serialized data</param>
        /// <returns></returns>
        bool TrySend(byte[] data);

        /// <summary>
        /// The event handler to be use invoked when data arrives from the stream
        /// </summary>
        event EventHandler<byte[]> DataArrived;
    }
}
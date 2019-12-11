using System;
using System.IO;
using System.Net.Mime;
using CloudNative.CloudEvents;
using Newtonsoft.Json;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Allows for the serialization and deserialization of types.
    /// Each implementation should handle its own optimization and fault tolerance.
    /// </summary>
    /// <remarks>Types of T may not be available at compile-time so special handling will need to be done for any attribute based configuration.</remarks>
    public interface ISpigotSerializer : IDisposable
    {
        /// <summary>
        /// Serializes the class to a byte array
        /// </summary>
        /// <typeparam name="T">The Type to be serialized. Must be a class and have a parameter-less constructor.</typeparam>
        /// <param name="dataToSerialize">The instance of the class to serialize</param>
        /// <returns></returns>
        byte[] Serialize<T>(T dataToSerialize) where T : class, new();

        /// <summary>
        /// Deserializes the raw byte array back to an instance of the class
        /// </summary>
        /// <typeparam name="T">The Type to be serialized. Must be a class and have a parameter-less constructor.</typeparam>
        /// <param name="serializedByteArray">The raw bytes received from the  <see cref="ISpigotStream"/> to be deserialized</param>
        /// <returns></returns>
        T Deserialize<T>(byte[] serializedByteArray) where T : class, new();

        /// <summary>
        /// Represents a MIME protocol Content-Type header
        /// </summary>
        ContentType ContentType { get; }
    }

    public class DefaultJsonSerializer : ISpigotSerializer
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public DefaultJsonSerializer()
        {
            _serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            _serializer.NullValueHandling = NullValueHandling.Ignore;
            _serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedByteArray) where T : class, new()
        {
            using (var s = new MemoryStream(serializedByteArray))
            using (var sr = new StreamReader(s))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return _serializer.Deserialize<T>(reader);
                }
            }
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T dataToSerialize) where T : class, new()
        {
            using (var mem = new MemoryStream())
            using (var textWriter = new StreamWriter(mem))
            {
                _serializer.Serialize(textWriter, dataToSerialize);
                textWriter.Flush();
                return mem.ToArray();
            }
        }

        public ContentType ContentType { get; } = new ContentType("application/json");

        public void Dispose()
        {
        }
    }
}
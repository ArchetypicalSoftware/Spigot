using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization;

namespace Archetypical.Software.Spigot
{

    /// <summary>
    /// A simple Data contract serializer 
    /// </summary>
    internal class DefaultDataContractSerializer : ISpigotSerializer
    {
        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();
        private readonly DataContractSerializerSettings _settings = new DataContractSerializerSettings();

        public DefaultDataContractSerializer()
        {
            _settings.IgnoreExtensionDataObject = true;
            _settings.SerializeReadOnlyTypes = true;
        }

        /// <inheritdoc />
        public T Deserialize<T>(byte[] serializedByteArray) where T : class, new()
        {
            var serializer = Serializers.GetOrAdd(typeof(T), new DataContractSerializer(typeof(T)));
            using (var stream = new MemoryStream(serializedByteArray))
            {
                return serializer.ReadObject(stream) as T;
            }
        }

        /// <inheritdoc />
        public byte[] Serialize<T>(T dataToSerialize) where T : class, new()
        {
            var serializer = Serializers.GetOrAdd(typeof(T), new DataContractSerializer(typeof(T), _settings));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, dataToSerialize);
                return stream.ToArray();
            }
        }
    }
}
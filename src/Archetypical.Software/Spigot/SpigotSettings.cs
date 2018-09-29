using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Archetypical.Software.Spigot
{
    public class SpigotSettings
    {
        internal Func<ISpigotSerializer> SerializerFactory;
        internal Func<ISpigotStream> StreamFactory;

        public SpigotSettings()
        {
            AddSerializer(new DefaultDataContractSerializer());
            AddStream(new LocalStream());
            ApplicationName = Environment.CommandLine;
        }
        public void AddSerializer(ISpigotSerializer serializer)
        {
            SerializerFactory = () => serializer;
        }
        public void AddStream(ISpigotStream stream)
        {
            StreamFactory = () => stream;
        }

        public string ApplicationName { get; set; }
    }

    class DefaultDataContractSerializer : ISpigotSerializer
    {
        internal class GenericResolver : DataContractResolver
        {
            public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
            {
                throw new NotImplementedException();
            }

            public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver,
                out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
            {
                throw new NotImplementedException();
            }
        }

        private static readonly ConcurrentDictionary<Type, DataContractSerializer> Serializers = new ConcurrentDictionary<Type, DataContractSerializer>();
        private DataContractSerializerSettings _settings = new DataContractSerializerSettings();
        public DefaultDataContractSerializer()
        {
            _settings.DataContractResolver = new GenericResolver();
            _settings.IgnoreExtensionDataObject = true;
            _settings.SerializeReadOnlyTypes = true;
            
        }
         public byte[] Serialize<T>(T dataToSerialize) where T : class, new()
         {
             var serializer = Serializers.GetOrAdd(typeof(T), new DataContractSerializer(typeof(T),_settings));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, dataToSerialize);
                return stream.ToArray();
            }
         }

        public T Deserialize<T>(byte[] serializedByteArray) where T : class, new()
        {
            var serializer = Serializers.GetOrAdd(typeof(T), new DataContractSerializer(typeof(T)));
            using (var stream = new MemoryStream(serializedByteArray))
            {
                return serializer.ReadObject(stream) as T;
            }
        }
    }

    class LocalStream : ISpigotStream {
        public bool TrySend(byte[] data)
        {
            DataArrived?.Invoke(this, data);
            return true;
        }

        public event EventHandler<byte[]> DataArrived;
    }
}
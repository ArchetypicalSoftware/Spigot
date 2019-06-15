using Archetypical.Software.Spigot;
using Newtonsoft.Json;
using Spigot.Samples.EventualConsistency.SynchronizedNodes.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes
{
    public class Demonstrator : IDemonstrator
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        static Demonstrator()
        {
            Archetypical.Software.Spigot.Spigot.Setup(settings =>
                {
                    settings.AddSerializer(new SimpleJsonSerializer());
                });
        }

        public void Go(TextWriter writer)
        {
            var remoteStoreEndpoints = new[]
            {
                new RemoteStoreEndpoint(), new RemoteStoreEndpoint(), new RemoteStoreEndpoint(),
            };

            var elements = new List<DataElement>();
            //add 100 elements
            for (var i = 0; i < 100; i++)
            {
                var element = GenerateDataElement();
                remoteStoreEndpoints[i % remoteStoreEndpoints.Length].AddOrUpdateDataElement(Clone(element));
                elements.Add(element);
            }

            for (var i = 0; i < 100; i++)
            {
                var element = elements[i];
                element.IntValue = (int)Random.Next();
                element.LastChanged = DateTime.Now;
                element.State = (State)Random.Next(0, 5);
                element.StringValue = Guid.NewGuid().ToString();
                remoteStoreEndpoints[i % remoteStoreEndpoints.Length].AddOrUpdateDataElement(Clone(element));
            }

            writer.WriteLine("Node states:");
            foreach (var node in remoteStoreEndpoints)
            {
                writer.WriteLine($"\tNode {node.Name} has {node.IndexedDataElements.Count} Elements with {node.Conflicts.Count} conflicts");
            }

            writer.WriteLine("Checking for inconsistencies....");
            for (var i = 0; i < 100; i++)
            {
                var element = elements[i];
                var remote =
                remoteStoreEndpoints[i % remoteStoreEndpoints.Length].GetDateElement(element.GuidIdentifier);
                var diffs = ItemComparer<DataElement>.Compare(element, remote);
                if (diffs.Any())
                {
                    writer.WriteLine($"\tInconsistencies on node {i} for DataElement::{element.GuidIdentifier}");
                }
            }

            writer.WriteLine("Sleeping for 5 sec...");
            Thread.Sleep(5000);

            writer.WriteLine("Checking for inconsistencies....");
            for (var i = 0; i < 100; i++)
            {
                var element = elements[i];
                var remote =
                    remoteStoreEndpoints[i % remoteStoreEndpoints.Length].GetDateElement(element.GuidIdentifier);
                var diffs = ItemComparer<DataElement>.Compare(element, remote);
                if (diffs.Any())
                {
                    writer.WriteLine($"\tInconsistencies on node {i} for DataElement::{element.GuidIdentifier}");
                }
            }
        }

        internal static DataElement Clone(DataElement element)
        {
            return JsonConvert.DeserializeObject<DataElement>(JsonConvert.SerializeObject(element));
        }

        private static DataElement GenerateDataElement()
        {
            return new DataElement
            {
                GuidIdentifier = Guid.NewGuid(),
                IntValue = (int)Random.Next(),
                LastChanged = DateTime.Now,
                State = (State)Random.Next(0, 5),
                StringValue = Guid.NewGuid().ToString()
            };
        }

        internal class SimpleJsonSerializer : ISpigotSerializer
        {
            public byte[] Serialize<T>(T dataToSerialize) where T : class, new()
            {
                return System.Text.Encoding.Default.GetBytes(JsonConvert.SerializeObject(dataToSerialize));
            }

            public T Deserialize<T>(byte[] serializedByteArray) where T : class, new()
            {
                return JsonConvert.DeserializeObject<T>(System.Text.Encoding.Default.GetString(serializedByteArray));
            }
        }

        public void Describe(TextWriter writer)
        {
            writer.WriteLine("This will demonstrate how nodes can be synchronized");
        }
    }
}
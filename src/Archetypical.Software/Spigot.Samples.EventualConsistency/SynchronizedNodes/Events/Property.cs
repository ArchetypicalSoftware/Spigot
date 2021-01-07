using Newtonsoft.Json;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    [JsonConverter(typeof(PropertyConverter))]
    public class Property
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public object Value { get; set; }
    }
}
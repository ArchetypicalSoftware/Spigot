using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    public class PropertyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var unboxed = (Property)value;
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(unboxed.Type));
            writer.WriteValue(unboxed.Type);
            writer.WritePropertyName(nameof(unboxed.Name));
            writer.WriteValue(unboxed.Name);
            writer.WritePropertyName(nameof(unboxed.Value));
            writer.WriteValue(JsonConvert.SerializeObject(unboxed.Value));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {// Load JObject from stream
            JObject jObject = JObject.Load(reader);

            var t = Type.GetType(jObject["Type"].Value<string>());
            var value = jObject["Value"].Value<string>();
            var name = jObject["Name"].Value<string>();
            // Populate the object properties

            return new Property
            {
                Type = t.FullName,
                Value = JsonConvert.DeserializeObject(value, t),
                Name = name
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace Oni2Xml.TypeData
{
    class TypeInstanceData
    {
    }

    class ObjectInstanceData : TypeInstanceData
    {
        public ObjectInstanceData(string name)
        {
            this.name = name;
        }

        public string name;
        public IDictionary<string, TypeInstanceData> fields = new Dictionary<string, TypeInstanceData>();
        public IDictionary<string, TypeInstanceData> properties = new Dictionary<string, TypeInstanceData>();
    }

    class PrimitiveInstanceData : TypeInstanceData
    {
        public PrimitiveInstanceData(object value)
        {
            this.value = value;
        }

        // Doesn't work, nothing to store the type on.  Allowing casting of reads instead.
        //// We need to know the exact type.  JSON.Net parses numbers as long or double.
        //[JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public object value;
    }

    class ArrayInstanceData : TypeInstanceData
    {
        public ArrayInstanceData()
        {
            this.values = new List<TypeInstanceData>();
        }

        public IList<TypeInstanceData> values;
    }

    class PairInstanceData : TypeInstanceData
    {
        public PairInstanceData(TypeInstanceData key, TypeInstanceData value) 
        {
            this.key = key;
            this.value = value;
        }

        public TypeInstanceData key;
        public TypeInstanceData value;
    }

    class DictionaryInstanceData : TypeInstanceData
    {
        public DictionaryInstanceData()
        {
            this.entries = new Dictionary<TypeInstanceData, TypeInstanceData>();
        }

        [JsonConverter(typeof(DictionaryInstanceEntriesConverter))]
        public IDictionary<TypeInstanceData, TypeInstanceData> entries;
    }

    class DictionaryInstanceEntriesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<TypeInstanceData, TypeInstanceData>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IDictionary<TypeInstanceData, TypeInstanceData> target;
            if (existingValue != null) {
                target = (IDictionary<TypeInstanceData, TypeInstanceData>) existingValue;
            }
            else
            {
                target = new Dictionary<TypeInstanceData, TypeInstanceData>();
            }

            var array = JArray.Load(reader);
            foreach(JObject obj in array)
            {
                target.Add(
                    (TypeInstanceData) serializer.Deserialize(obj["key"].CreateReader()),
                    (TypeInstanceData) serializer.Deserialize(obj["value"].CreateReader())
                );
            }
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var target = (IDictionary<TypeInstanceData, TypeInstanceData>) value;

            writer.WriteStartArray();
            foreach (var pair in target)
            {
                writer.WriteStartObject();

                // Note: We specify the serialized type as TypeInstanceData so that the serializer
                //  identifies the real type as being a subclass, which causes it to write out the type.

                writer.WritePropertyName("key");
                serializer.Serialize(writer, pair.Key, typeof(TypeInstanceData));
                writer.WritePropertyName("value");
                serializer.Serialize(writer, pair.Value, typeof(TypeInstanceData));
                writer.WriteEndObject();
            }
            writer.WriteEnd();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cuscino
{
    public class ViewKeysConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            var keyValue = reader.Value.ToString();
            if (!keyValue.StartsWith("["))
            {
                return new string[] {keyValue};
            }
            else
            {
                // Load JObject from stream
                JObject jObject = JObject.Load(reader);

                // Create target object based on JObject
                string[] target = {""};

                // Populate the object properties
                serializer.Populate(jObject.CreateReader(), target);

                return target;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class CouchViewResultItem<T>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("key")]
        //[JsonConverter(typeof(ViewKeysConverter))]
        public string[] Key { get; set; }
        [JsonProperty("value")]
        public T Value { get; set; }
    }
    public class CouchViewResult<T>
    {
        [JsonProperty("total_rows")]
        public int TotalRows { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("rows")]
        public IEnumerable<CouchViewResultItem<T>> Items { get; set; }

        public IEnumerable<T> Rows
        {
            get { return Items.Select(x => x.Value); }
        }

        public CouchViewResult()
        {
            Items = new List<CouchViewResultItem<T>>();
        }
    }
}

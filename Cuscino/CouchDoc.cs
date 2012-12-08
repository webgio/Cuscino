using System;
using Newtonsoft.Json;

namespace Cuscino
{
    public abstract class CouchDoc
    {
        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public String Id { get; set; }

        [JsonProperty("_rev", NullValueHandling= NullValueHandling.Ignore)]
        public string Revision { get; set; }

        [JsonProperty("type", NullValueHandling= NullValueHandling.Ignore)]
        public string Type { get; set; } 

        protected CouchDoc()
        {
            //this.Id = Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}

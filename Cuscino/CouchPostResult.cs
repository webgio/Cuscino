using Newtonsoft.Json;

namespace Cuscino
{
    public class CouchRequestResult
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("rev")]
        public string Revision { get; set; }

    }
}

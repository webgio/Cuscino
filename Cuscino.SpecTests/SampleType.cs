using System;
using Newtonsoft.Json;

namespace Cuscino.SpecTests
{
    public class SampleType: CouchDoc
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cuscino
{
    public class CouchViewResultItem<T>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
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
        public IEnumerable<CouchViewResultItem<T>> Rows { get; set; }

        public CouchViewResult()
        {
            Rows = new List<CouchViewResultItem<T>>();
        }
    }
}

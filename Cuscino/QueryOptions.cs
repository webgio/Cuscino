using System;
using System.Collections.Generic;

namespace Cuscino
{
    public class QueryOptions
    {
        public bool Descending { get; set; }
        public string Key { get; set; }
        public string Startkey { get; set; }
        public string Endkey { get; set; }
        public int Limit { get; set; }
        public int Skip { get; set; }
        public bool IncludeDocs { get; set; }
        public bool Group { get; set; }

        public QueryOptions()
        {
            Key = string.Empty;
            Startkey = string.Empty;
            Endkey = string.Empty;
        }

        public Dictionary<string,string> GetCriteria()
        {
            var criterias = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(Startkey) == false)
                criterias.Add("startkey", Startkey);
            if (String.IsNullOrEmpty(Endkey) == false)
                criterias.Add("endkey", Endkey);
            if (Descending)
                criterias.Add("descending", "true");

            return criterias;
        }
    }
}
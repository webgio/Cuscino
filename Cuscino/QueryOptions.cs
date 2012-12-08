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
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cuscino
{
    public class CouchClientAsync: CouchClientBase, ICouchClientAsync
    {
        public CouchClientAsync(string database)
            : this("http://localhost:5984", database, "", "")
        {
        }

        public CouchClientAsync(string host, string database, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
            this.db = database;
            ValidateClient();
        }

        public async Task<string[]> GetDatabases()
        {
            var result = await DoRequestAsync(host + "/_all_dbs", "GET");

            JArray d = JArray.Parse(result);
            var list = new List<string>();
            foreach (var db in d)
                list.Add(db.ToString());
            return (list.ToArray());
        }

        public async Task<bool> DatabaseExists()
        {
            var dbs = await this.GetDatabases();
            return dbs.Contains(db);
        }

        internal async Task<string> DoRequestAsync(string url, string method)
        {
            return await DoRequestAsync(url, method, null);
        }

        private async Task<string> DoRequestAsync(string url, string method, string postdata)
        {
            using (var wc = new WebClient())
            {
                var creds = new CredentialCache { { new Uri(this.host), "Basic", new NetworkCredential(this.username, this.password) } };
                wc.Credentials = creds;
                
                if (postdata != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postdata.ToString(CultureInfo.InvariantCulture));
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    return System.Text.Encoding.UTF8.GetString(await wc.UploadDataTaskAsync(url, method, bytes));
                }

                if (method == "PUT")
                {
                    return await wc.UploadStringTaskAsync(url, "PUT", string.Empty);
                }

                return System.Text.Encoding.UTF8.GetString(await wc.DownloadDataTaskAsync(url));
            }
        }
        
        public async Task<IEnumerable<dynamic>> GetDynamicViewAsync(string viewdef, string criteria)
        {
            var uri = host + "/" + db + "/_design/" + viewdef;
            if (string.IsNullOrEmpty(criteria) == false) uri += "?" + criteria;
            var stringList = await DoRequestAsync(uri, "GET");

            var jss = new JavaScriptSerializer();

            var data = jss.Deserialize<dynamic>(stringList);

            return data["rows"];
        }



        public async Task<IEnumerable<T>> GetViewAsync<T>(string viewdef, string criteria) where T : CouchDoc
        {
            var uri = host + "/" + db + "/_design/" + viewdef;
            if (string.IsNullOrEmpty(criteria) == false) uri += "?" + criteria;
            var stringList = await DoRequestAsync(uri, "GET");

            JObject searchResultJson = JObject.Parse(stringList);

            // get JSON result objects into a list
            IList<JToken> results = searchResultJson["rows"].Children().ToList();

            // serialize JSON results into .NET objects
            IList<T> searchResults = new List<T>();
            foreach (JToken result in results)
            {
                var searchResult = JsonConvert.DeserializeObject<T>(result["value"].ToString());
                searchResults.Add(searchResult);
            }

            return searchResults;
        }

        public async Task<IEnumerable<T>> GetViewAsync<T>(CouchQuery couchQuery) where T : CouchDoc
        {
            var uri = host + "/" + db + "/_design/" + couchQuery.GetUri();
            if (string.IsNullOrEmpty(couchQuery.GetCriteria()) == false) uri += "?" + couchQuery.GetCriteria();

            var stringList = await DoRequestAsync(uri, "GET");

            JObject searchResultJson = JObject.Parse(stringList);

            // get JSON result objects into a list
            IList<JToken> results = searchResultJson["rows"].Children().ToList();

            // serialize JSON results into .NET objects
            IList<T> searchResults = new List<T>();
            foreach (JToken result in results)
            {
                var searchResult = JsonConvert.DeserializeObject<T>(result["value"].ToString());
                searchResults.Add(searchResult);
            }

            return searchResults;
        }
        
        public async Task<T> GetEntityAsync<T>(string docid) where T : CouchDoc
        {
            var jsondata = await DoRequestAsync(this.host + "/" + db + "/" + docid, "GET");
            var entity = JsonConvert.DeserializeObject<T>(jsondata);
            return entity;
        }

        public async Task<IEnumerable<dynamic>> GetDynamicViewAsync(CouchQuery couchQuery)
        {
            var uri = host + "/" + db + "/_design/" + couchQuery.GetUri();
            if (string.IsNullOrEmpty(couchQuery.GetCriteria()) == false) uri += "?" + couchQuery.GetCriteria();

            var stringList = await DoRequestAsync(uri, "GET");

            var jss = new JavaScriptSerializer();

            var data = jss.Deserialize<dynamic>(stringList);

            return data["rows"];
        }

        public async Task<CouchRequestResult> CreateDatabaseIfNotExistsAsync()
        {
            var dbExists = await DatabaseExists();
            if (dbExists)
                return new CouchRequestResult { Ok=true };
            string result = await DoRequestAsync(host + "/" + db, "PUT");
            if (result.Contains("{\"ok\":true}") != true)
                throw new ApplicationException("Failed to create database: " + result);
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        public Task<CouchRequestResult> DeleteDatabaseIfExistsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<CouchRequestResult> PostDocumentAsync(string content)
        {
            throw new System.NotImplementedException();
        }

        public Task<CouchRequestResult> DeleteDocumentAsync(string docid, string revision)
        {
            throw new System.NotImplementedException();
        }

        public Task<CouchRequestResult> PostEntityAsync<T>(T entity) where T : CouchDoc
        {
            throw new System.NotImplementedException();
        }

        public Task<T> GetDocumentAsEntityAsync<T>(string docid) where T : CouchDoc
        {
            throw new System.NotImplementedException();
        }

        public Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string viewPath)
        {
            throw new System.NotImplementedException();
        }

        public Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string viewPath, QueryOptions queryOptions)
        {
            throw new System.NotImplementedException();
        }
    }
}
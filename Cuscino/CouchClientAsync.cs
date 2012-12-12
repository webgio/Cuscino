using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        internal async Task<string> DoRequestAsync(string url, string method, string postdata)
        {
            return await DoRequestAsync(url, method, postdata, null);
        }

        private async Task<string> DoRequestAsync(string url, string method, string postdata, Dictionary<string,string> criterias)
        {
            using (var wc = new WebClient())
            {
                var creds = new CredentialCache { { new Uri(this.host), "Basic", new NetworkCredential(this.username, this.password) } };
                wc.Credentials = creds;

                if (criterias != null)
                {
                    foreach (var criteria in criterias)
                    {
                        wc.QueryString.Add(criteria.Key, criteria.Value);
                    }
                }
                
                if (postdata != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postdata.ToString(CultureInfo.InvariantCulture));
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    return System.Text.Encoding.UTF8.GetString(await wc.UploadDataTaskAsync(url, method, bytes));
                }

                if (method == "PUT" || method == "DELETE")
                {
                    return await wc.UploadStringTaskAsync(url, method, string.Empty);
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

        public async Task<CouchRequestResult> DeleteDatabaseIfExistsAsync()
        {
            var dbExists = await DatabaseExists();
            if (!dbExists)
                return new CouchRequestResult { Ok = true };
            string result = await DoRequestAsync(host + "/" + db, "DELETE");
            if (result.Contains("{\"ok\":true}") != true)
                throw new ApplicationException("Failed to delete database: " + result);
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        public async Task<CouchRequestResult> PostDocumentAsync(string content)
        {
            var result = await DoRequestAsync(host + "/" + db, "POST", content);
            var postResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return postResult;
        }

        public async Task<CouchRequestResult> DeleteDocumentAsync(string docid, string revision)
        {
            var result = await DoRequestAsync(this.host + "/" + db + "/" + docid + "?rev=" + revision, "DELETE");
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        public async Task<CouchRequestResult> PostEntityAsync<T>(T entity) where T : CouchDoc
        {
            var content = JsonConvert.SerializeObject(
                entity,
                Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var result = await DoRequestAsync(host + "/" + db, "POST", content);
            var postResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            entity.Id = postResult.Id;
            return postResult;
        }

        public async Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string view)
        {
            return await QueryViewAsEntityAsync<T>(view, new QueryOptions());
        }

        public async Task<CouchViewResult<T>> QueryViewAsEntityAsync<T>(string view, QueryOptions queryOptions)
        {
            CouchViewResult<T> result = new CouchViewResult<T>();

            var uri = host + "/" + db + "/_design/application/_view/" + view;

            var jsondata = await DoRequestAsync(uri, "GET", null, queryOptions.GetCriteria());

            JObject viewResultJson = JObject.Parse(jsondata);

            IList<JToken> rows = viewResultJson["rows"].Children().ToList();

            // serialize JSON results into .NET objects
            var items = new List<CouchViewResultItem<T>>();
            foreach (JToken item in rows)
            {
                var valueJson = item["value"].ToString();
                var value = JsonConvert.DeserializeObject<T>(valueJson);
                var keysJson = item["key"].ToString();
                string[] keys;
                if (!keysJson.StartsWith("["))
                    keys = new string[] { keysJson };
                else
                {
                    keys = JsonConvert.DeserializeObject<string[]>(keysJson);
                }
                var resultItem = new CouchViewResultItem<T>
                    {
                        Id = item["id"].ToString(),
                        Key = keys,
                        Value = value
                    };
                items.Add(resultItem);
            }

            result.Items = items;
            result.Offset = int.Parse(viewResultJson["offset"].ToString());
            result.TotalRows = int.Parse(viewResultJson["total_rows"].ToString());
            
            //result = JsonConvert.DeserializeObject<CouchViewResult<T>>(jsondata);

            return result;
        }


        public async Task<string> GetDocumentAsync(string docid)
        {
            return await DoRequestAsync(this.host + "/" + db + "/" + docid, "GET");
        }
    }
}
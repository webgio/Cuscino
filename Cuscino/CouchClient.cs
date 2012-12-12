using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cuscino
{
    public interface ICouchClient
    {
        CouchRequestResult CreateDatabaseIfNotExists();

        CouchRequestResult DeleteDatabaseIfExists();
        
        CouchRequestResult PostDocument(string content);

        CouchRequestResult DeleteDocument(string docid, string revision);

        CouchRequestResult PostEntity<T>(T entity) where T : CouchDoc;

        T GetDocumentAsEntity<T>(string docid) where T : CouchDoc;
        
        CouchViewResult<T> QueryViewAsEntity<T>(string viewPath);

        CouchViewResult<T> QueryViewAsEntity<T>(string viewPath, QueryOptions queryOptions);
    }

    /// <summary>
    /// A simple wrapper class for the CouchDB HTTP API. No
    /// initialisation is necessary, just create an instance and
    /// call the appropriate methods to interact with CouchDB.
    /// All methods throw exceptions when things go wrong.
    /// </summary>
    public class CouchClient : CouchClientBase, ICouchClient
    {

        public CouchClient(string database)
            : this("http://localhost:5984", database, "", "")
        {
        }

        public CouchClient(string host, string database, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
            this.db = database;
            ValidateClient();
        }
        
        /// <summary>
        /// Get a list of database on the server.
        /// </summary>
        /// <param name="server">The server URL</param>
        /// <returns>A string array containing the database names
        /// </returns>
        public string[] GetDatabases()
        {
            var result = DoRequest(host + "/_all_dbs", "GET");

            JArray d = JArray.Parse(result);
            return (d.Select(x => x.ToString()).ToArray());
        }

        public bool DatabaseExists()
        {
            var dbs = this.GetDatabases();
            return dbs.Contains(db);
        }

        /// <summary>
        /// Get the document count for the given database.
        /// </summary>
        /// <param name="server">The server URL</param>
        /// <param name="db">The database name</param>
        /// <returns>The number of documents in the database</returns>
        public int CountDocuments()
        {
            // I don't know a more efficient way of doing this at
            // present other than getting a list of all documents...
            string result = DoRequest(host + "/" + db + "/_all_docs", "GET");

            JObject d = JObject.Parse(result);
            int count = d["rows"].Count();
            return count;
        }


        /// <summary>
        /// Get information on all the documents in the given database.
        /// </summary>
        /// <param name="server">The server URL</param>
        /// <param name="db">The database name</param>
        /// <returns>An array of DocInfo instances</returns>
        public CouchDocInfo[] GetAllDocuments()
        {
            string result = DoRequest(host + "/" + db + "/_all_docs", "GET");

            List<CouchDocInfo> list = new List<CouchDocInfo>();
            JObject d = JObject.Parse(result);
            foreach (var row in d["rows"])
            {
                CouchDocInfo doc = new CouchDocInfo();
                doc.ID = row["id"].ToString();
                doc.Revision = row["value"]["rev"].ToString();
                list.Add(doc);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Create a new database.
        /// </summary>
        /// <param name="server">The server URL</param>
        /// <param name="db">The database name</param>
        public CouchRequestResult CreateDatabaseIfNotExists()
        {
            if (DatabaseExists())
                return new CouchRequestResult{Ok = true};
            string result = DoRequest(host + "/" + db, "PUT");
            if (result.Contains("{\"ok\":true}") != true)
                throw new ApplicationException("Failed to create database: " + result);
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        /// <summary>
        /// Delete a database
        /// </summary>
        /// <param name="server">The server URL</param>
        /// <param name="db">The name of the database to delete</param>
        public CouchRequestResult DeleteDatabaseIfExists()
        {
            if (DatabaseExists() == false)
                return new CouchRequestResult { };
            string result = DoRequest(host + "/" + db, "DELETE");
            if (result.Contains("{\"ok\":true}") != true)
                throw new ApplicationException("Failed to delete database: " + result);
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        public string ExecTempView(string viewdef)
        {
            return DoRequest(host + "/" + db + "/_temp_view", "POST", viewdef);
        }

        public string GetView(string viewdef, string criteria)
        {
            var uri = host + "/" + db + "/_design/" + viewdef;
            if (string.IsNullOrEmpty(criteria) == false) uri += "?" + criteria;
            return DoRequest(uri, "GET");
        }

        public CouchRequestResult PostDocument(string content)
        {
            var result = DoRequest(host + "/" + db, "POST", content);
            var postResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return postResult;
        }

        public CouchRequestResult PostEntity<T>(T entity) where T : CouchDoc
        {
            var content = JsonConvert.SerializeObject(
                entity, 
                Formatting.Indented, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var result = DoRequest(host + "/" + db, "POST", content);
            var postResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return postResult;
        }

        public string GetDocument(string docid)
        {
            return DoRequest(this.host + "/" + db + "/" + docid, "GET");
        }

        public T GetDocumentAsEntity<T>(string docid) where T : CouchDoc
        {
            var jsondata = DoRequest(this.host + "/" + db + "/" + docid, "GET");
            var entity = JsonConvert.DeserializeObject<T>(jsondata);
            return entity;
        }

        public dynamic GetDocumentAsDynamic(string docid)
        {
            var jsondata = DoRequest(this.host + "/" + db + "/" + docid, "GET");
            var entity = JsonConvert.DeserializeObject<ExpandoObject>(jsondata);
            return entity;
        }
        
        public CouchRequestResult DeleteDocument(string docid, string revision)
        {
            var result = DoRequest(this.host + "/" + db + "/" + docid + "?rev=" + revision, "DELETE");
            var reqResult = JsonConvert.DeserializeObject<CouchRequestResult>(result);
            return reqResult;
        }

        /// <summary>
        /// Internal helper to make an HTTP request and return the
        /// response. Throws an exception in the event of any kind
        /// of failure. Overloaded - use the other version if you
        /// need to post data with the request.
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="method">The method, e.g. "GET"</param>
        /// <returns>The server's response</returns>
        private string DoRequest(string url, string method)
        {
            return DoRequest(url, method, null);
        }

        /// <summary>
        /// Internal helper to make an HTTP request and return the
        /// response. Throws an exception in the event of any kind
        /// of failure. Overloaded - use the other version if no
        /// post data is required.
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="method">The method, e.g. "GET"</param>
        /// <param name="postdata">Data to be posted with the request,
        /// or null if not required.</param>
        /// <returns>The server's response</returns>
        private string DoRequest(string url, string method, string postdata)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = method;
            
            CredentialCache creds = new CredentialCache();
            creds.Add(new Uri(this.host), "Basic", new NetworkCredential(this.username, this.password));
            req.Credentials = creds;

            if (postdata != null)
            {
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(postdata.ToString());
                req.ContentLength = bytes.Length;
                req.ContentType = "application/json";
                using (Stream ps = req.GetRequestStream())
                {
                    ps.Write(bytes, 0, bytes.Length);
                }
            }

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            string result;
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        
        public CouchViewResult<T> QueryViewAsEntity<T>(string view)
        {
            return QueryViewAsEntity<T>(view, new QueryOptions());
        }

        public CouchViewResult<T> QueryViewAsEntity<T>(string view, QueryOptions queryOptions)
        {
            var result = new CouchViewResult<T>();

            var uri = host + "/" + db + "/_design/application/_view/" + view;

            var jsondata = DoRequest(uri, "GET");
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
    }
}

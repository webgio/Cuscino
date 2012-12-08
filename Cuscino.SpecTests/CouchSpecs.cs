using System;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    public class CouchSpecs
    {
        protected static string DbName;
        protected static ICouchClient CouchClient;

        Establish context = () =>
            {
                DbName = "testing_cuscino"; // +DateTime.UtcNow.Ticks;
                CouchClient = new Cuscino.CouchClient("http://localhost.:5984", DbName, "", "");
                CouchClient.DeleteDatabaseIfExists();
                CouchClient.CreateDatabaseIfNotExists();
            };

        // Cleanup after = () => CouchClient.DeleteDatabaseIfExists();
    }

    public class PostToCouchSpecs : CouchSpecs
    {
        protected static CouchRequestResult couchPostResult;
        protected static BlogPost newBlogPost;
    }
}
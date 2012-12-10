using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Cuscino.SpecTests.AsyncSpecs
{
    public class AsyncCouchSpecs
    {
        protected static string DbName;
        protected static ICouchClientAsync CouchClient;

        Establish context = () =>
        {
            DbName = "testing_cuscino_async";
            CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
            CouchClient.DeleteDatabaseIfExistsAsync().Await();
            CouchClient.CreateDatabaseIfNotExistsAsync().Await();
        };
    }
}

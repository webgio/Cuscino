using System.Collections.Generic;
using System.Text;
using Machine.Specifications;

namespace Cuscino.SpecTests.AsyncSpecs
{
    [Subject(typeof (CouchClientAsync))]
    public class when_getting_database_list
    {
        protected static string DbName;
        protected static CouchClientAsync CouchClient;

        Establish context = () =>
            {
                DbName = "testing_cuscino"; // +DateTime.UtcNow.Ticks;
                CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
                //CouchClient.DeleteDatabaseIfExists();
                //CouchClient.CreateDatabaseIfNotExists();
            };

        static string[] databases;

        Because of = () => { databases = CouchClient.GetDatabases().Await<string[]>(); };

        It should_show_a_list_of_databases = () =>
                                             databases.ShouldNotBeEmpty();
    }

    
    [Subject(typeof (CouchClientAsync))]
    public class when_creating_new_database
    {
        protected static string DbName;
        protected static CouchClientAsync CouchClient;

        Establish context = () =>
        {
            DbName = "testi"; // +DateTime.UtcNow.Ticks;
            CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
            //CouchClient.DeleteDatabaseIfExists();
            //CouchClient.CreateDatabaseIfNotExists();
        };

        static CouchRequestResult couchPostResult;

        Because of = () =>
            {
                couchPostResult = CouchClient.CreateDatabaseIfNotExistsAsync().Await();
            };

        It should_return_ok_status = () => 
            couchPostResult.Ok.ShouldBeTrue();
    }

}
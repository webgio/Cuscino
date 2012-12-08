using Machine.Specifications;

namespace Cuscino.SpecTests.AsyncSpecs
{
    public class CouchAsyncSpecs
    {
        protected static string DbName;
        protected static ICouchClientAsync CouchClient;

        Establish context = () =>
            {
                DbName = "testing_cuscino"; // +DateTime.UtcNow.Ticks;
                CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
                //CouchClient.DeleteDatabaseIfExists();
                //CouchClient.CreateDatabaseIfNotExists();
            };

        // Cleanup after = () => CouchClient.DeleteDatabaseIfExists();
    }
}
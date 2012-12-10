using System.Collections.Generic;
using System.Linq;
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
            DbName = "test_create";
            CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
        };

        static CouchRequestResult couchPostResult;

        Because of = () =>
            {
                couchPostResult = CouchClient.CreateDatabaseIfNotExistsAsync().Await();
            };

        It should_return_ok_status = () => 
            couchPostResult.Ok.ShouldBeTrue();
    }

    [Subject(typeof (CouchClientAsync))]
    public class when_deleting_a_database
    {
        protected static string DbName;
        protected static CouchClientAsync CouchClient;

        Establish context = () =>
        {
            DbName = "test_delete";
            CouchClient = new Cuscino.CouchClientAsync("http://localhost.:5984", DbName, "", "");
            CouchClient.CreateDatabaseIfNotExistsAsync().Await();
        };

        static CouchRequestResult couchPostResult;

        Because of = () =>
        {
            couchPostResult = CouchClient.DeleteDatabaseIfExistsAsync().Await();
        };

        It should_return_ok_status = () =>
            couchPostResult.Ok.ShouldBeTrue();
        
    }

    [Subject(typeof(CouchClientAsync))]
    public class when_posting_a_document : AsyncCouchSpecs
    {
        static string document;
        static CouchRequestResult couchPostResult;

        Establish context = () =>
                    document = @"{
                               ""_id"": ""testing_a_document"",
                               ""name"": ""How are you""
                            }";

        Because of = () =>
                     couchPostResult = CouchClient.PostDocumentAsync(document).Await();

        It Should_save_the_document = () =>
                                      couchPostResult.Ok.ShouldBeTrue();
    }

    [Subject(typeof(CouchClientAsync))]
    public class when_sending_delete_request : AsyncCouchSpecs
    {
        static CouchRequestResult couchPostResult;
        static CouchRequestResult couchDeleteResult;

        Establish context = () =>
        {
            var document = @"{
                               ""_id"": ""testing_a_document"",
                               ""name"": ""How are you"",
                               ""created"":""2012-12-07T13:02:18.9840504Z""
                            }";
            couchPostResult = CouchClient.PostDocumentAsync(document).Await();
        };

        Because of = () =>
        {
            couchDeleteResult = CouchClient.DeleteDocumentAsync("testing_a_document", couchPostResult.Revision).Await();
        };

        It should_remove_the_doc = () => couchDeleteResult.Ok.ShouldBeTrue();
    }

    [Subject(typeof(CouchClientAsync))]
    public class when_posting_an_entity_with_the_id_generated_by_the_server : AsyncCouchSpecs
    {
        protected static CouchRequestResult couchPostResult;
        protected static BlogPost newBlogPost;

        Establish context = () =>
        {
            newBlogPost = new BlogPost
            {
                Author = "Giorgio",
                Title = "Testing Cuscino",
                Text = "This is a test"
            };
        };

        Because of = () =>
                     couchPostResult = CouchClient.PostEntityAsync(newBlogPost).Await();

        It Should_save_the_entity = () =>
                                    couchPostResult.Ok.ShouldBeTrue();

        It Should_get_back_a_not_empty_id = () =>
                                            couchPostResult.Id.ShouldNotBeEmpty();
    }

    [Subject(typeof(CouchClientAsync))]
    public class when_getting_a_entity : AsyncCouchSpecs
    {
        static BlogPost blogPost;
        Establish context = () =>
                            CouchClient.PostEntityAsync(new BlogPost
                            {
                                Id = "testing null prop",
                                Author = null,
                                Title = "Testing Cuscino",
                                Text = "This is a test"
                            }).Await();

        Because of =()=> {
                             blogPost = CouchClient.GetEntityAsync<BlogPost>("testing null prop").Await();
        };

        It Should_return_it = () =>
            blogPost.ShouldNotBeNull();
    }
    
    [Subject(typeof(CouchClientAsync))]
    public class when_asking_for_a_view : AsyncCouchSpecs
    {
        static CouchViewResult<SampleType> results;

        Establish context = () =>
        {
            var view = @"{
                               ""_id"": ""_design/application"",
                               ""views"": {
                                   ""created"": {
                                       ""map"": ""function(doc) {if(doc.created) { var dateIndex = Date.parse(doc.created); emit([dateIndex], doc);}}""
                                   }
                               }
                            }";
            var document1 = @"{
                               ""_id"": ""testing_a_document"",
                               ""name"": ""How are you"",
                               ""created"":""2012-12-05T13:02:18.9840504Z""
                            }";
            var document2 = @"{
                               ""_id"": ""testing_a_document2"",
                               ""name"": ""How are you"",
                               ""created"":""2012-12-07T13:02:18.9840504Z""
                            }";
            CouchClient.PostDocumentAsync(view).Await();
            CouchClient.PostDocumentAsync(document1).Await();
            CouchClient.PostDocumentAsync(document2).Await();
        };

        Because of = () =>
                     results = CouchClient.QueryViewAsEntityAsync<SampleType>("created").Await();

        It should_return_a_CouchViewResult_of_SampleType = () =>
                                                           results.ShouldBeOfType<CouchViewResult<SampleType>>();

        It should_return_two_elements = () =>
                                        results.Items.Count().ShouldEqual(2);

        It should_return_elements_with_correct_ids = () =>
        {
            (results.Items.ElementAt(0).Id).ShouldEqual("testing_a_document");
            (results.Items.ElementAt(1).Id).ShouldEqual("testing_a_document2");
        };
    }
}
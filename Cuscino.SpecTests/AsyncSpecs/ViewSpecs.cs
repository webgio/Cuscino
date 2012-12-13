using System.Linq;
using Machine.Specifications;

namespace Cuscino.SpecTests.AsyncSpecs
{
    public abstract class AsyncViewCouchSpecs: AsyncCouchSpecs
    {
        protected static CouchViewResult<SampleType> results;
        Establish context = () =>
        {
            var view = @"{
                               ""_id"": ""_design/application"",
                               ""views"": {
                                   ""created"": {
                                       ""map"": ""function(doc) {if(doc.created) { var dateIndex = Date.parse(doc.created); emit(dateIndex, null);}}""
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
    }

    [Subject(typeof(CouchClient))]
    public class Asking_for_a_view : AsyncViewCouchSpecs
    {
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

    [Subject(typeof(CouchClient))]
    public class Asking_for_a_view_with_docs_included : AsyncViewCouchSpecs
    {
        Because of = () =>
                     results = CouchClient
                     .QueryViewAsEntityAsync<SampleType>(
                        "created", 
                        new QueryOptions{IncludeDocs=true})
                     .Await();
        
        It should_return_two_elements = () =>
                                        results.Items.Count().ShouldEqual(2);

        It should_return_docs = () =>
        {
            (results.Items.ElementAt(0).Doc).ShouldNotBeNull();
            (results.Items.ElementAt(1).Doc).ShouldNotBeNull();
        };
    }
}
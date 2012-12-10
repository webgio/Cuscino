using System.Linq;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject(typeof(CouchClient))]
    public class Asking_for_a_view: CouchSpecs
    {
        static CouchViewResult<SampleType> results;

        Establish context = () =>
            {
                var view = @"{
                               ""_id"": ""_design/application"",
                               ""views"": {
                                   ""created"": {
                                       ""map"": ""function(doc) {if(doc.created) { var dateIndex = Date.parse(doc.created); emit(dateIndex, doc);}}""
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
                CouchClient.PostDocument(view);
                CouchClient.PostDocument(document1);
                CouchClient.PostDocument(document2);
            };
        
        Because of = () =>
                     results = CouchClient.QueryViewAsEntity<SampleType>("created");

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
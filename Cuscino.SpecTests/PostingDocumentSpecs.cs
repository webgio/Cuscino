using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject(typeof(CouchClient))]
    public class Posting_a_document: CouchSpecs
    {
        static string document;
        static CouchRequestResult couchPostResult;

        Establish context = () =>
                    document = @"{
                               ""_id"": ""testing_a_document"",
                               ""name"": ""How are you""
                            }";

        Because of = () =>
                     couchPostResult = CouchClient.PostDocument(document);

        It Should_save_the_document = () =>
                                      couchPostResult.Ok.ShouldBeTrue();
    }
}

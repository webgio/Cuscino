using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject(typeof(CouchClient))]
    public class when_sending_delete_request: CouchSpecs
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
                couchPostResult = CouchClient.PostDocument(document);
            };

        Because of = () =>
        {
            couchDeleteResult = CouchClient.DeleteDocument("testing_a_document", couchPostResult.Revision);
        };

        It should_remove_the_doc = () => couchDeleteResult.Ok.ShouldBeTrue();
    }
}

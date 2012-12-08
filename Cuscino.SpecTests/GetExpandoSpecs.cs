using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject(typeof(CouchClient))]
    [Ignore]
    public class Getting_a_document_as_dynamic_type: CouchSpecs
    {
        static dynamic result;
        Establish context = () =>
            {
                var document = @"{
                               ""_id"": ""testing_a_dynamic_get"",
                               ""Created"":""2012-12-05T00:00:00"",
                               ""Name"": ""Giorgio""
                            }";
                CouchClient.PostDocument(document);
            };

        //Because of = () =>
        //             result = CouchClient.GetDocumentAsDynamic("testing_a_dynamic_get");

        It Should_retreive_a_dynamic_object = () => 
            ShouldExtensionMethods.ShouldBeOfType<dynamic>(result) ;

        It Should_have_the_properties_correctly_setted = () =>
        {
            ((string)result._id).ShouldEqual("testing_a_dynamic_get");
            ((string)result.Name).ShouldEqual("Giorgio");
            ((DateTime)result.Created).ShouldEqual(new DateTime(2012,12,5));
        };
                                                         
    }
}

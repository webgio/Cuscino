using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject(typeof(CouchClient))]
    public class Retrieving_an_entity : CouchSpecs
    {
        Establish context = () =>
                            CouchClient.PostEntity(new BlogPost
                                {
                                    Id = "testing null prop",
                                    Author = "Giorgio",
                                    Title = "Testing Cuscino",
                                    Text = "This is a test"
                                });

        It Should_return_it;
    }

    [Subject("Retrieving an entity")]
    public class with_a_missing_property : CouchSpecs
    {
        Establish context = () =>
                            CouchClient.PostEntity(new BlogPost
                                {
                                    Id = "testing null prop",
                                    Author = null,
                                    Title = "Testing Cuscino",
                                    Text = "This is a test"
                                });

        It Should_return_it_with_the_property_as_null;
    }
}
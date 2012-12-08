using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests
{
    [Subject("Posting an entity")]
    public class with_the_id_generated_by_the_server : PostToCouchSpecs
    {
        Establish context = () =>
                            newBlogPost = new BlogPost
                                {
                                    Author = "Giorgio",
                                    Title = "Testing Cuscino",
                                    Text = "This is a test"
                                };

        Because of = () =>
                     couchPostResult = CouchClient.PostEntity(newBlogPost);

        It Should_save_the_entity = () =>
                                    couchPostResult.Ok.ShouldBeTrue();

        It Should_get_back_a_not_empty_id = () =>
                                            couchPostResult.Id.ShouldNotBeEmpty();
    }

    [Subject("Posting an entity")]
    public class with_a_property_set_to_null : PostToCouchSpecs
    {
        Establish context = () =>
                            newBlogPost = new BlogPost
                                {
                                    Id = "testing null prop",
                                    Author = null,
                                    Title = "Testing Cuscino",
                                    Text = "This is a test"
                                };

        Because of = () =>
                     couchPostResult = CouchClient.PostEntity(newBlogPost);

        It Should_save_the_entity = () =>
                                    couchPostResult.Ok.ShouldBeTrue();
    }
}
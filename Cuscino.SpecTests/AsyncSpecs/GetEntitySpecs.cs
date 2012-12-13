using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Cuscino.SpecTests.AsyncSpecs
{
    [Subject(typeof(CouchClientAsync))]
    public class when_getting_non_existant_doc: AsyncCouchSpecs
    {
        static BlogPost doc;
        
        Because of = () =>
            {
                doc = CouchClient.GetEntityAsync<BlogPost>("thisdoesntexist").Await();
            };

        It should_return_null = () => 
            doc.ShouldBeNull();
    }
}

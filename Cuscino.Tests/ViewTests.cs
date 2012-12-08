using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using SharpTestsEx;

namespace Cuscino.Tests
{
    public class Equipment : CouchDoc
    {
        public string Name { get; set; }

        public Equipment()
        {
            Type = "Equipment";
        }
    }

    [TestFixture]
    public class ViewTests
    {
        private string dbname;
        private CouchClient client;

        [SetUp]
        public void SetupSingleTest()
        {
            dbname = "prova" + DateTime.UtcNow.Ticks;
            // to make this test work, you should configure couchdb to bind 0.0.0.0
            // not using 127.0.0.1 or localhost to enable capturing traffic with fiddler2
            client = new Cuscino.CouchClient("http://localhost.:5984", dbname, "", "");
            client.CreateDatabaseIfNotExists();
        }

        [TearDown]
        public void Clenup()
        {
            client.DeleteDatabaseIfExists();
        }


        [Test]
        public void CanCreateView()
        {
            var jsonApplication = @"{
               ""_id"": ""_design/application"",
               ""views"": {
                   ""equipmentname"": {
                       ""map"": ""function(doc) {if(doc.type == 'Equipment') { emit(doc.Name, null);  }  }""
                   }
               }
            }";

            client.PostDocument(jsonApplication);
        }

//        [Test]
//        public void GivenAViewThatReturnsOriginalEntity_ICanQuery()
//        {

//            var jsonApplication = @"{
//                           ""_id"": ""_design/application"",
//                           ""views"": {
//                               ""equipmentname"": {
//                                   ""map"": ""function(doc) {if(doc.type == 'Equipment') { emit(doc.Name, doc);  }  }""
//                               }
//                           }
//                        }";

//            client.PostDocument(jsonApplication);

//            var equip = new Equipment() { Name = "prova", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            client.PostEntity(equip);

//            var results = client.GetDynamicViewAsync("application/_view/equipmentname", "").Result.ToList();
//            results.Should().Have.Count.EqualTo(1);
//            var result = results[0];
//            var name = result["value"]["Name"];
//            Extensions.Should((object)name).Be.EqualTo("prova");
//        }

//        [Test]
//        public void GivenAViewThatReturnsString_ICanQuery()
//        {
//            var jsonApplication = @"{
//                           ""_id"": ""_design/application"",
//                           ""views"": {
//                               ""equipmentname"": {
//                                   ""map"": ""function(doc) {if(doc.type == 'Equipment') { emit(doc.Name, doc.Name);  }  }""
//                               }
//                           }
//                        }";

//            client.PostDocument(jsonApplication);

//            var equip = new Equipment() { Name = "prova", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            client.PostEntity(equip);

//            var results = client.GetDynamicViewAsync("application/_view/equipmentname", "").Result.ToList();
//            results.Should().Not.Be.Empty();
//            results.Should().Have.Count.EqualTo(1);
//            var result = results[0];
//            var name = result["value"];
//            ((object)name).Should().Be.EqualTo("prova");
//        }

//        [Test]
//        public void GivenAViewThatReturnsString_ICanQueryUsingCouchQuery()
//        {
//            var jsonApplication = @"{
//                           ""_id"": ""_design/application"",
//                           ""views"": {
//                               ""equipmentname"": {
//                                   ""map"": ""function(doc) {if(doc.type == 'Equipment') { emit(doc.Name, doc);  }  }""
//                               }
//                           }
//                        }";

//            client.PostDocument(jsonApplication);
//            var equip = new Equipment() { Name = "Screwdriver", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            var equip2 = new Equipment() { Name = "Pizza hoven", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            client.PostEntity(equip);
//            client.PostEntity(equip2);

//            var query = CouchQuery.OnView("equipmentname")
//                .StartingFrom("pazza")
//                .EndingIn("puzza");
//            var results = client.GetDynamicViewAsync(query).Result.ToList();

//            results.Should().Not.Be.Empty();
//            results.Should().Have.Count.EqualTo(1);
//            var result = results[0];
//            var name = result["value"]["Name"];
//            ((object)name).Should().Be.EqualTo(equip2.Name);
//        }


//        [Test]
//        public void GivenAViewThatReturnsGenreric_ICanQueryUsingCouchQuery()
//        {
//            var jsonApplication = @"{
//                           ""_id"": ""_design/application"",
//                           ""views"": {
//                               ""equipmentname"": {
//                                   ""map"": ""function(doc) {if(doc.type == 'Equipment') { emit(doc.Name, doc);  }  }""
//                               }
//                           }
//                        }";

//            client.PostDocument(jsonApplication);
//            var equip = new Equipment() { Name = "Screwdriver", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            var equip2 = new Equipment() { Name = "Pizza hoven", Id = Guid.NewGuid().ToString().Replace("-", "") };
//            client.PostEntity(equip);
//            client.PostEntity(equip2);

//            var query = CouchQuery.OnView("equipmentname")
//                .StartingFrom("pazza")
//                .EndingIn("puzza");
//            var results = client.GetViewAsync<Equipment>(query).Result.ToList();

//            results.Should().Not.Be.Empty();
//            results.Should().Have.Count.EqualTo(1);
//            results[0].Name.Should().Be.EqualTo(equip2.Name);
//        }
    }
}

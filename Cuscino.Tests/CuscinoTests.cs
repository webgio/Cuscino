using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using SharpTestsEx;

namespace Cuscino.Tests
{
    [TestFixture]
    public class CuscinoTests
    {
        private string dbname;
        private CouchClient client;

        [SetUp]
        public void SetupSingleTest()
        {
            dbname = "prova" + DateTime.UtcNow.Ticks;
            // to make this test work, you should configure couchdb to bind 0.0.0.0
            // not using 127.0.0.1 or localhost to enable capturing traffic with fiddler2
            client = new Cuscino.CouchClient("http://localhost:5984", dbname, "", "");
            client.CreateDatabaseIfNotExists();
        }

        [TearDown]
        public void Clenup()
        {
            client.DeleteDatabaseIfExists();
        }

        [Test]
        public void CanCreateAndDeleteDb()
        {
            var dbs = client.GetDatabases();
            dbs.Should().Contain(dbname);
        }
        
        [Test]
        public void CanPostEntity()
        {
            var equip = new Equipment() { Name = "prova", Id = Guid.NewGuid().ToString().Replace("-", "") };
            client.PostEntity(equip);

            var ondb = client.GetDocumentAsEntity<Equipment>(equip.Id);
            ondb.Name.Should().Be.EqualTo("prova");
        }
    }
}

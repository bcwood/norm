using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;

namespace Norm.Tests
{
    [TestFixture]
    public class ConnectionExtensionTests
    {
        private const int VALID_ID = 1;
        private const int INVALID_ID = 99999;

        private SqlConnection _connection;
        
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Norm.Tests"].ConnectionString);
            _connection.Open();
        }

        [TearDown]
        public void Teardown()
        {
            _connection.Close();
            _connection.Dispose();
        }

        #endregion // Setup/Teardown

        #region Select

        [Test]
        [Category("Select")]
        public void Select_ById_ReturnsNotNull()
        {
            Person p = _connection.Select<Person>(new { Id = VALID_ID }).SingleOrDefault();

            Assert.IsNotNull(p);
        }

        [Test]
        [Category("Select")]
        public void Select_ById_RecordNotExists_ReturnsNull()
        {
            Person p = _connection.Select<Person>(new { Id = INVALID_ID }).SingleOrDefault();

            Assert.IsNull(p);
        }

        [Test]
        [Category("Select")]
        public void Select_All_ReturnsNonEmptyList()
        {
            IEnumerable<Person> list = _connection.Select<Person>();

            Assert.IsNotEmpty(list);
        }

        [Test]
        [Category("Select")]
        public void Select_ById_500Iterations()
        {
            for (int i = 1; i <= 500; i++)
            {
                Person p = _connection.Select<Person>(new { Id = i }).SingleOrDefault();
                Assert.IsNotNull(p);
            }
        }

        #endregion // Select

        #region Insert

        [Test]
        [Category("Insert")]
        public void Insert_ReturnsNonZeroId()
        {
            var p = Person.Random();
            p.Id = _connection.Insert(p);

            Assert.IsTrue(p.Id > 0);
        }

        [Test]
        [Category("Insert")]
        public void Insert_GetById_ReturnsEqualObject()
        {
            var p = Person.Random();
            p.Id = _connection.Insert(p);

            var p2 = _connection.Select<Person>(new { Id = p.Id }).SingleOrDefault();

            Assert.AreEqual(p, p2);
        }

        #endregion // Insert

        #region Update

        [Test]
        [Category("Update")]
        public void Update_ReturnsTrue()
        {
            Person p = _connection.Select<Person>(new { Id = VALID_ID }).SingleOrDefault();

            p.FirstName = TestHelpers.GetRandomString(20);
            p.LastName = TestHelpers.GetRandomString(20);

            Assert.IsTrue(_connection.Update(p));
        }

        [Test]
        [Category("Update")]
        public void Update_RecordNotExists_ReturnsFalse()
        {
            var p = Person.Random();
            p.Id = INVALID_ID;
            
            Assert.IsFalse(_connection.Update(p));
        }

        [Test]
        [Category("Update")]
        public void Update_GetById_ReturnsEqualObject()
        {
            Person p = _connection.Select<Person>(new { Id = VALID_ID }).SingleOrDefault();

            p.FirstName = TestHelpers.GetRandomString(20);
            p.LastName = TestHelpers.GetRandomString(20);

            _connection.Update(p);

            Person p2 = _connection.Select<Person>(new { Id = VALID_ID }).SingleOrDefault();

            Assert.AreEqual(p, p2);
        }

        #endregion // Update

        #region Delete

        [Test]
        [Category("Delete")]
        public void Delete_ReturnsTrue()
        {
            var p = Person.Random();
            p.Id = _connection.Insert(p);

            Assert.IsTrue(_connection.Delete(p));
        }

        [Test]
        [Category("Delete")]
        public void Delete_RecordNotExists_ReturnsFalse()
        {
            var p = Person.Random();
            p.Id = INVALID_ID;

            Assert.IsFalse(_connection.Delete(p));
        }

        [Test]
        [Category("Delete")]
        public void Delete_GetById_ReturnsNull()
        {
            var p = Person.Random();
            p.Id = _connection.Insert(p);
            _connection.Delete(p);

            var p2 = _connection.Select<Person>(new { Id = p.Id }).SingleOrDefault();

            Assert.IsNull(p2);
        }

        #endregion // Delete
    }
}
